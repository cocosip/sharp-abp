"""Merge Coverlet JSON by source location; never average suite percentages."""

import argparse
import json
from pathlib import Path


def merge_reports(paths):
    modules = {}
    for path in paths:
        for assembly, documents in json.loads(path.read_text(encoding="utf-8-sig")).items():
            name = Path(assembly.replace("\\", "/")).stem
            if not name.startswith("SharpAbp.") or name.endswith((".Tests", ".AllTests", ".TestBase")):
                continue
            module = modules.setdefault(name, {"lines": {}, "branches": {}})
            for document, classes in documents.items():
                document = document.replace("\\", "/")
                for class_name, methods in classes.items():
                    for method_name, method in methods.items():
                        for line, hits in method["Lines"].items():
                            key = (document, int(line))
                            module["lines"][key] = module["lines"].get(key, False) or hits > 0
                        for branch in method["Branches"]:
                            key = (document, class_name, method_name, branch["Line"],
                                   branch["Offset"], branch["EndOffset"], branch["Path"], branch["Ordinal"])
                            module["branches"][key] = module["branches"].get(key, False) or branch["Hits"] > 0
    return modules


def production_projects(repo):
    roots = [repo / "framework" / "src", *sorted((repo / "modules").glob("*/src"))]
    return {project.stem: project.relative_to(repo).as_posix()
            for root in roots for project in root.rglob("*.csproj")}


def percentage(covered, total):
    return 100 * covered / total if total else None


def summarize(modules, projects, threshold, non_executable=()):
    rows = []
    for name, project in sorted(projects.items()):
        module = modules.get(name)
        no_code = name in non_executable
        measured = module is not None or no_code
        lines = module["lines"] if module is not None else {}
        branches = module["branches"] if module is not None else {}
        rate = percentage(sum(lines.values()), len(lines))
        rows.append({"assembly": name, "project": project, "measured": measured,
                     "no_executable_lines": no_code,
                     "lines_covered": sum(lines.values()), "lines_total": len(lines),
                     "line_percent": rate, "branches_covered": sum(branches.values()),
                     "branches_total": len(branches),
                     "branch_percent": percentage(sum(branches.values()), len(branches)),
                     "passes": measured and (rate is None or rate >= threshold)})
    covered = sum(row["lines_covered"] for row in rows)
    total = sum(row["lines_total"] for row in rows)
    all_measured = bool(rows) and all(row["measured"] for row in rows)
    rate = percentage(covered, total)
    return {"threshold": threshold, "all_projects_measured": all_measured,
            "measured_line_percent": percentage(covered, total),
            "lines_covered": covered, "lines_total": total,
            "passes": all_measured and rate is not None and rate >= threshold, "projects": rows}


def markdown(summary):
    def display(value):
        return "N/A" if value is None else f"{value:.2f}%"

    lines = ["# Local automated test coverage", "",
             f"Required repository line coverage: {summary['threshold']:g}%. "
             "Individual assemblies below this target are listed separately.", "",
             f"Measured source lines: {summary['lines_covered']}/{summary['lines_total']} "
             f"({display(summary['measured_line_percent'])}).", "",
             "Unmeasured projects are gaps, not passes. The measured percentage does not represent "
             "the whole repository when projects are absent. Local ABP/SQLite integration tests "
             "are included; this is not external-service acceptance evidence.", "",
             "| Production assembly | Lines | Branches | Status |",
             "| --- | ---: | ---: | --- |"]
    for row in summary["projects"]:
        status = "UNMEASURED" if not row["measured"] else ("PASS" if row["passes"] else "BELOW TARGET")
        if row["no_executable_lines"]:
            status = "NO EXECUTABLE LINES (PDB)"
        lines.append(f"| {row['assembly']} | {display(row['line_percent'])} | "
                     f"{display(row['branch_percent'])} | {status} |")
    return "\n".join(lines) + "\n"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("results", type=Path, help="A fresh directory from one complete test run")
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--threshold", type=float, default=85)
    parser.add_argument("--report-only", action="store_true", help="Record a baseline without enforcing the gate")
    args = parser.parse_args()
    if not 0 <= args.threshold <= 100:
        parser.error("threshold must be between 0 and 100")
    paths = sorted(args.results.rglob("coverage.json"))
    if not paths:
        parser.error("No Coverlet coverage.json files found; use coverage.runsettings")
    symbols_path = args.results / "symbols.json"
    symbols = json.loads(symbols_path.read_text(encoding="utf-8-sig")) if symbols_path.exists() else {}
    summary = summarize(merge_reports(paths), production_projects(args.repo.resolve()), args.threshold,
                        {name for name, count in symbols.items() if count == 0})
    (args.results / "summary.json").write_text(json.dumps(summary, indent=2) + "\n", encoding="utf-8")
    (args.results / "summary.md").write_text(markdown(summary), encoding="utf-8")
    print(f"Coverage report: {args.results / 'summary.md'}")
    print(f"Projects meeting target: {sum(row['passes'] for row in summary['projects'])}/{len(summary['projects'])}")
    return 0 if args.report_only or summary["passes"] else 1


if __name__ == "__main__":
    raise SystemExit(main())
