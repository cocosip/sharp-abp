﻿namespace SharpAbp.MinId
{
    /// <summary>
    /// Represents a generic response wrapper for MinId operations that indicates success or failure.
    /// </summary>
    public class MinIdResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Gets or sets a message providing additional information about the operation result.
        /// For successful operations, this may be null or empty. For failed operations, this contains error details.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a successful response with the specified data.
        /// </summary>
        /// <typeparam name="T">The type of data to include in the response.</typeparam>
        /// <param name="data">The data to include in the successful response.</param>
        /// <returns>A MinIdResponse containing the provided data with Success set to true.</returns>
        public static MinIdResponse<T> Successful<T>(T data)
        {
            return new MinIdResponse<T>()
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed response with the specified error message.
        /// </summary>
        /// <typeparam name="T">The type of data that would be expected in a successful response.</typeparam>
        /// <param name="message">The error message explaining why the operation failed.</param>
        /// <returns>A MinIdResponse with Success set to false and the provided error message.</returns>
        public static MinIdResponse<T> Failed<T>(string message)
        {
            return new MinIdResponse<T>()
            {
                Success = false,
                Message = message
            };
        }
    }

    /// <summary>
    /// Represents a generic response wrapper for MinId operations that includes data.
    /// This class extends MinIdResponse to add a typed data property.
    /// </summary>
    /// <typeparam name="T">The type of data contained in the response.</typeparam>
    public class MinIdResponse<T> : MinIdResponse
    {
        /// <summary>
        /// Gets or sets the data returned by the operation.
        /// This property is only populated for successful responses.
        /// </summary>
        public T Data { get; set; }
    }
}