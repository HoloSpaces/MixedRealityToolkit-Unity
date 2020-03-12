﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Response to a REST Call.
    /// </summary>
    public struct Response
    {
        /// <summary>
        /// Was the REST call successful?
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        /// Response body from the resource.
        /// </summary>
        public string ResponseBody => cachedResponseBody ?? (cachedResponseBody = responseBodyAction?.Invoke());
        private string cachedResponseBody;
        private System.Func<string> responseBodyAction;

        /// <summary>
        /// Response data from the resource.
        /// </summary>
        public byte[] ResponseData => responseDataAction?.Invoke();
        private System.Func<byte[]> responseDataAction;

        /// <summary>
        /// Response code from the resource.
        /// </summary>
        public long ResponseCode { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Response(bool successful, string responseBody, byte[] responseData, long responseCode)
        {
            Successful = successful;
            responseBodyAction = responseBody == null ? null : (System.Func<string>)(() => responseBody);
            cachedResponseBody = null;
            responseDataAction = responseData == null ? null : (System.Func<byte[]>)(() => responseData);
            ResponseCode = responseCode;
        }

        public Response(bool successful, System.Func<string> responseBodyAction, System.Func<byte[]> responseDataAction, long responseCode)
        {
            Successful = successful;
            this.responseBodyAction = responseBodyAction;
            cachedResponseBody = null;
            this.responseDataAction = responseDataAction;
            ResponseCode = responseCode;
        }
    }
}