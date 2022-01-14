﻿// 1.0
using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Windows.Forms;

namespace youtube_dl_gui {

    /// <summary>
    /// This class will control the Errors that get reported in try statements.
    /// </summary>
    class ErrorLog {
        public static string ComputerVersionInformation;

        /// <summary>
        /// Assembles the computer information one time so exceptions do not require parsing through the management objects.
        /// </summary>
        public static void AssembleComputerVersionInformation() {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            ManagementObject info = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
            ComputerVersionInformation =
                $"Version: {info.Properties["Version"].Value} " +
                $"Service Pack Major: {info.Properties["ServicePackMajorVersion"].Value} " +
                $"Service Pack Minor: {info.Properties["ServicePackMinorVersion"].Value} " +
                $"System Caption: {info.Properties["Caption"].Value}";
        }

        /// <summary>
        /// Reports any web errors that are caught
        /// </summary>
        /// <param name="WebException">The WebException that was caught</param>
        /// <param name="url">The URL that (might-have) caused the problem</param>
        public static DialogResult Report(WebException ReceivedException, string WebsiteAddress) {
            if (!Config.Settings.Errors.suppressErrors) {
                string CustomDescriptionBuffer = string.Empty;
                bool UseCustomDescription = false;

                using (frmException ExceptionDisplay = new frmException()) {
                    ExceptionDisplay.ReportedWebException = ReceivedException;
                    ExceptionDisplay.FromLanguage = false;
                    ExceptionDisplay.WebAddress = WebsiteAddress;

                    switch (ReceivedException.Status) {
                        #region NameResolutionFailure
                        case WebExceptionStatus.NameResolutionFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nName resolution failure" +
                                            "\nThe name resolver service could not resolve the host name.";
                            break;
                        #endregion
                        #region ConnectFailure
                        case WebExceptionStatus.ConnectFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nConnection failure" +
                                            "\nThe remote service point could not be contacted at the transport level.";
                            break;
                        #endregion
                        #region RecieveFailure
                        case WebExceptionStatus.ReceiveFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nRecieve failure" +
                                            "\nA complete response was not received from the remote server.";
                            break;
                        #endregion
                        #region SendFailure
                        case WebExceptionStatus.SendFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nSend failure" +
                                            "\nA complete response could not be sent to the remote server.";
                            break;
                        #endregion
                        #region PipelineFailure
                        case WebExceptionStatus.PipelineFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nPipeline failure" +
                                            "\nThe request was a piplined request and the connection was closed before the response was received.";
                            break;
                        #endregion
                        #region RequestCanceled
                        case WebExceptionStatus.RequestCanceled:
                            return DialogResult.OK;
                        #endregion
                        #region ProtocolError
                        case WebExceptionStatus.ProtocolError:
                            HttpWebResponse WebResponse = (HttpWebResponse)ReceivedException.Response;

                            if (WebResponse != null) {
                                UseCustomDescription = true;
                                switch ((int)WebResponse.StatusCode) {
                                    #region StatusCodes
                                    #region default / unspecified
                                    default:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned " + WebResponse.StatusCode.ToString() +
                                            "\n" + WebResponse.StatusDescription.ToString();
                                        break;
                                    #endregion

                                    #region 301 Moved / Moved permanently
                                    case 301:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 301 - Moved / Moved permanently" +
                                            "\nThe requested information has been moved to the URI specified in the Location header.";
                                        break;
                                    #endregion

                                    #region 400 Bad request
                                    case 400:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 400 - Bad request" +
                                            "\nThe request could not be understood by the server.";
                                        break;
                                    #endregion

                                    #region 401 Unauthorized
                                    case 401:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 401 - Unauthorized" +
                                            "\nThe requested resource requires authentication.";
                                        break;
                                    #endregion

                                    #region 402 Payment required
                                    case 402:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 402 - Payment required" +
                                            "\nPayment is required to view this content.\nThis status code isn't natively used.";
                                        break;
                                    #endregion

                                    #region 403 Forbidden
                                    case 403:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 403 - Forbidden" +
                                            "\nYou do not have permission to view this file.";
                                        break;
                                    #endregion

                                    #region 404 Not found
                                    case 404:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 404 - Not found" +
                                            "\nThe file does not exist on the server.";
                                        break;
                                    #endregion

                                    #region 405 Method not allowed
                                    case 405:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 405 - Method not allowed" +
                                            "\nThe request method (GET) is not allowed on the requested resource.";
                                        break;
                                    #endregion

                                    #region 406 Not acceptable
                                    case 406:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 406 - Not acceptable" +
                                            "\nThe client has indicated with Accept headers that it will not accept any of the available representations from the resource.";
                                        break;
                                    #endregion

                                    #region 407 Proxy authentication required
                                    case 407:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 407 - Proxy authentication required" +
                                            "\nThe requested proxy requires authentication.";
                                        break;
                                    #endregion

                                    #region 408 Request timeout
                                    case 408:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 408 - Request timeout" +
                                            "\nThe client did not send a request within the time the server was expection the request.";
                                        break;
                                    #endregion

                                    #region 409 Conflict
                                    case 409:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 409 - Conflict" +
                                            "\nThe request could not be carried out because of a conflict on the server.";
                                        break;
                                    #endregion

                                    #region 410 Gone
                                    case 410:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 410 - Gone" +
                                            "\nThe requested resource is no longer available.";
                                        break;
                                    #endregion

                                    #region 411 Length required
                                    case 411:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 410 - Length required" +
                                            "\nThe required Content-length header is missing.";
                                        break;
                                    #endregion

                                    #region 412 Precondition failed
                                    case 412:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 412 - Precondition failed" +
                                            "\nA condition set for this request failed, and the request cannot be carried out.";
                                        break;
                                    #endregion

                                    #region 413 Request entity too large
                                    case 413:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 413 - Request entity too large" +
                                            "\nThe request is too large for the server to process.";
                                        break;
                                    #endregion

                                    #region 414 Request uri too long
                                    case 414:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 414 - Request uri too long" +
                                            "\nThe uri is too long.";
                                        break;
                                    #endregion

                                    #region 415 Unsupported media type
                                    case 415:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 415 - Unsupported media type" +
                                            "\nThe request is an unsupported type.";
                                        break;
                                    #endregion

                                    #region 416 Requested range not satisfiable
                                    case 416:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 416 - Requested range not satisfiable" +
                                            "\nThe range of data requested from the resource cannot be returned.";
                                        break;
                                    #endregion

                                    #region 417 Expectation failed
                                    case 417:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 417 - Expectation failed" +
                                            "\nAn expectation given in an Expect header could not be met by the server.";
                                        break;
                                    #endregion

                                    #region 426 Upgrade required
                                    case 426:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 426 - Upgrade required" +
                                            "\nNo information is available about this error code.";
                                        break;
                                    #endregion

                                    #region 500 Internal server error
                                    case 500:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 500 - Internal server error" +
                                            "\nAn error occured on the server.";
                                        break;
                                    #endregion

                                    #region 501 Not implemented
                                    case 501:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 501 - Not implemented" +
                                            "\nThe server does not support the requested function.";
                                        break;
                                    #endregion

                                    #region 502 Bad gateway
                                    case 502:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 502 - Bad gateway" +
                                            "\nThe proxy server Received a bad response from another proxy or the origin server.";
                                        break;
                                    #endregion

                                    #region 503  Service unavailable
                                    case 503:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 503 - Service unavailable" +
                                            "\nThe server is temporarily unavailable, likely due to high load or maintenance.";
                                        break;
                                    #endregion

                                    #region 504 Gateway timeout
                                    case 504:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 504 - Gateway timeout" +
                                            "\nAn intermediate proxy server timed out while waiting for a response from another proxy or the origin server.";
                                        break;
                                    #endregion

                                    #region 505 Http version not supported
                                    case 505:
                                        CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nThe address returned 505 - Http version not supported" +
                                            "\nThe requested HTTP version is not supported by the server.";
                                        break;
                                        #endregion
                                        #endregion
                                }
                            }
                            break;
                        #endregion
                        #region ConnectionClosed
                        case WebExceptionStatus.ConnectionClosed:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nConnection closed" +
                                            "\nThe connection was prematurely closed.";
                            break;
                        #endregion
                        #region TrustFailure
                        case WebExceptionStatus.TrustFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nTrust failure" +
                                            "\nA server certificate could not be validated.";
                            break;
                        #endregion
                        #region SecureChannelFailure
                        case WebExceptionStatus.SecureChannelFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nSecure channel failure" +
                                            "\nAn error occurred while establishing a connection using SSL.";
                            break;
                        #endregion
                        #region ServerProtocolViolation
                        case WebExceptionStatus.ServerProtocolViolation:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nServer protocol violation" +
                                            "\nThe server response was not a valid HTTP response.";
                            break;
                        #endregion
                        #region KeepAliveFailure
                        case WebExceptionStatus.KeepAliveFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nKeep alive failure" +
                                            "\nThe connection for a request that specifies the Keep-alive header was closed unexpectedly.";
                            break;
                        #endregion
                        #region Pending
                        case WebExceptionStatus.Pending:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nPending" +
                                            "\nAn internal asynchronous request is pending.";
                            break;
                        #endregion
                        #region Timeout
                        case WebExceptionStatus.Timeout:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nTimeout" +
                                            "\nNo response was received during the time-out period for a request.";
                            break;
                        #endregion
                        #region ProxyNameResolutionFailure
                        case WebExceptionStatus.ProxyNameResolutionFailure:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nProxy name resolution failure" +
                                            "\nThe name resolver service could not resolve the proxy host name.";
                            break;
                        #endregion
                        #region UnknownError
                        case WebExceptionStatus.UnknownError:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nUnknown error" +
                                            "\nAn exception of unknown type has occurred.";
                            break;
                        #endregion
                        #region MessageLengthLimitExceeded
                        case WebExceptionStatus.MessageLengthLimitExceeded:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nMessage length limit exceeded" +
                                            "\nA message was received that exceeded the specified limit when sending a request or receiving a response from the server.";
                            break;
                        #endregion
                        #region CacheEntryNotFound
                        case WebExceptionStatus.CacheEntryNotFound:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nCache entry not found" +
                                            "\nThe specified cache entry was not found.";
                            break;
                        #endregion
                        #region RequestProhibitedByCachePolicy
                        case WebExceptionStatus.RequestProhibitedByCachePolicy:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nRequest prohibited by cache policy" +
                                            "\nThe request was not permitted by the cache policy.";
                            break;
                        #endregion
                        #region RequestProhibitedByProxy
                        case WebExceptionStatus.RequestProhibitedByProxy:
                            UseCustomDescription = true;
                            CustomDescriptionBuffer += "A WebException occured at " + WebsiteAddress +
                                            "\n\nRequest prohibited by proxy" +
                                            "\nThis request was not permitted by the proxy.";
                            break;
                        #endregion
                    }

                    if (UseCustomDescription) {
                        CustomDescriptionBuffer += ReceivedException.InnerException + "\n\nStackTrace:\n" +
                                                   ReceivedException.StackTrace;
                    }

                    if (UseCustomDescription) { WriteToFile(CustomDescriptionBuffer); }
                    else { WriteToFile(ReceivedException.ToString()); }

                    ExceptionDisplay.SetCustomDescription = UseCustomDescription;
                    ExceptionDisplay.CustomDescription = CustomDescriptionBuffer;
                    return ExceptionDisplay.ShowDialog();
                }
            }
            else return DialogResult.OK;
        }

        /// <summary>
        /// Reports any general exceptions that are caught
        /// </summary>
        /// <param name="Exception">The Exception that was caught</param>
        public static DialogResult Report(Exception ReceivedException, bool IsWriteToFile = false) {
            if (!Config.Settings.Errors.suppressErrors) {
                if (!IsWriteToFile) WriteToFile(ReceivedException.ToString());
                using (frmException ExceptionDisplay = new frmException()) {
                    ExceptionDisplay.ReportedException = ReceivedException;
                    ExceptionDisplay.FromLanguage = false;
                    return ExceptionDisplay.ShowDialog();
                }
            }
            else return DialogResult.OK;

        }

        /// <summary>
        /// Reports a decimal parsing exception that may get caught.
        /// </summary>
        /// <param name="ReceivedException">The <seealso cref="DecimalParsingException"/> received.</param>
        public static DialogResult Report(DecimalParsingException ReceivedException) {
            if (!Config.Settings.Errors.suppressErrors) {
                WriteToFile(ReceivedException.ToString());
                using (frmException ExceptionDisplay = new frmException()) {
                    ExceptionDisplay.ReportedDecimalParsingException = ReceivedException;
                    ExceptionDisplay.FromLanguage = false;
                    return ExceptionDisplay.ShowDialog();
                }
            }
            else return DialogResult.OK;
        }

        /// <summary>
        /// Reports a api parsing exception that may get caught.
        /// </summary>
        /// <param name="ReceivedException">The <seealso cref="ApiParsingException"/> received.</param>
        public static DialogResult Report (ApiParsingException ReceivedException) {
            if (!Config.Settings.Errors.suppressErrors) {
                WriteToFile(ReceivedException.ToString());
                using (frmException ExceptionDisplay = new frmException()) {
                    ExceptionDisplay.ReportedApiParsingException = ReceivedException;
                    ExceptionDisplay.FromLanguage = false;
                    return ExceptionDisplay.ShowDialog();
                }
            }
            else return DialogResult.OK;
        }

        /// <summary>
        /// Writes the error to a .log file in the working directory.
        /// </summary>
        /// <param name="Buffer">The data that will be written to the log file</param>
        private static void WriteToFile(string LogData) {
            if (Config.Settings.Errors.logErrors && !Program.IsDebug) {
                try {
                    string FileName = $"\\error_{DateTime.Now}.log";
                    System.IO.File.WriteAllText(FileName, LogData);
                }
                catch (Exception ex) { Report(ex, true); }
            }
        }

    }

    /// <summary>
    /// A exception that occurs when decimal.TryParse returns false.
    /// </summary>
    [Serializable]
    public class DecimalParsingException : Exception {
        public string ExtraInfo = "No extra info provided.";
        public DecimalParsingException() : base("No message has been provided.") { }
        public DecimalParsingException(string message) : base(message) { }
        public DecimalParsingException(string message, string extraInfo) : base(message) { ExtraInfo = extraInfo; }
    }

    /// <summary>
    /// An exception that occurs when parsing an API fails at a critical point.
    /// </summary>
    [Serializable]
    public class ApiParsingException : Exception {
        public string ExtraInfo = "No extra info provided.";
        public string ApiUrl = "No API URL provided.";
        public ApiParsingException(string url) : base("No message has been provided.") { ApiUrl = url; }
        public ApiParsingException(string message, string url) : base(message) { ApiUrl = url; }
        public ApiParsingException(string message, string url, string extraInfo) : base(message) { ApiUrl = url; ExtraInfo = extraInfo; }
    }

}