import json
from pathlib import Path
import tempfile
import unittest

from coverage_report import markdown, merge_reports, production_projects, summarize


class CoverageReportTests(unittest.TestCase):
    def test_merges_complementary_hits_without_double_counting_shared_source(self):
        with tempfile.TemporaryDirectory() as directory:
            paths = []
            for index, hits in enumerate(((1, 0), (0, 1))):
                method = {"Lines": {"10": hits[0], "11": hits[1]}, "Branches": [
                    {"Line": 10, "Offset": 0, "EndOffset": 1, "Path": branch,
                     "Ordinal": branch, "Hits": hit} for branch, hit in enumerate(hits)]}
                data = {"SharpAbp.Example.dll": {"src/Example.cs": {"Example": {"Run": method}}},
                        "SharpAbp.Example.Tests.dll": {}, "ThirdParty.dll": {}}
                path = Path(directory) / f"{index}.json"
                path.write_text(json.dumps(data))
                paths.append(path)
            modules = merge_reports(paths)

        self.assertEqual(["SharpAbp.Example"], list(modules))
        summary = summarize(modules, {"SharpAbp.Example": "src/Example.csproj"}, 85)
        self.assertEqual(2, summary["lines_total"])
        self.assertEqual(2, summary["lines_covered"])
        self.assertEqual(100, summary["projects"][0]["branch_percent"])
        self.assertTrue(summary["passes"])

    def test_unreferenced_production_project_fails_even_with_full_measured_coverage(self):
        summary = summarize({"SharpAbp.Tested": {"lines": {("file", 1): True}, "branches": {}}},
                            {"SharpAbp.Tested": "a.csproj", "SharpAbp.Missing": "b.csproj"}, 85)
        self.assertEqual(100, summary["measured_line_percent"])
        self.assertFalse(summary["all_projects_measured"])
        self.assertFalse(summary["passes"])
        self.assertIn("UNMEASURED", markdown(summary))

    def test_threshold_uses_unrounded_rates(self):
        lines = {("file", line): line < 849 for line in range(1000)}
        summary = summarize({"SharpAbp.Low": {"lines": lines, "branches": {}}},
                            {"SharpAbp.Low": "low.csproj"}, 85)
        self.assertFalse(summary["passes"])

    def test_symbols_can_identify_marker_only_assemblies_without_hiding_untested_code(self):
        summary = summarize({}, {"SharpAbp.Marker": "marker.csproj", "SharpAbp.Missing": "missing.csproj"},
                            85, {"SharpAbp.Marker"})
        rows = {row["assembly"]: row for row in summary["projects"]}
        self.assertTrue(rows["SharpAbp.Marker"]["measured"])
        self.assertTrue(rows["SharpAbp.Marker"]["no_executable_lines"])
        self.assertFalse(rows["SharpAbp.Missing"]["measured"])
        self.assertFalse(summary["passes"])

    def test_inventory_includes_every_src_project_but_not_tests_or_apps(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            for name in ("framework/src/SharpAbp.Core/Core.csproj", "modules/a/src/App/App.csproj",
                         "modules/a/test/Tests.csproj", "modules/a/app/Host.csproj"):
                path = root / name
                path.parent.mkdir(parents=True, exist_ok=True)
                path.touch()
            self.assertEqual({"Core", "App"}, set(production_projects(root)))


if __name__ == "__main__":
    unittest.main()
