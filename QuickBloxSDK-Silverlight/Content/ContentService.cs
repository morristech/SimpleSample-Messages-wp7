using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using QuickBloxSDK_Silverlight.Core;
using QuickBloxSDK_Silverlight.users;

namespace QuickBloxSDK_Silverlight.Content
{
    public class ContentService
    {
        public delegate void ContentServiceHandler(ContentServiceEventArgs Args);

        public event ContentServiceHandler ContentServiceEvent;

        public string CustomServerAddr
        { get; set; }
       
        public bool IsOnline
        {
            get;
            set;
        }

        public int OwnerId
        { get; set; }

        public User user
        { get; set; }

        
        public Session session
        { get; set; }

         /// <summary>
        /// Connection context
        /// </summary>
        private ConnectionContext Сontext;

        /// <summary>
        /// Designer
        /// </summary>
        /// <param name="context"></param>
        public ContentService(ConnectionContext context)
        {
            Сontext = context;
            this.Сontext.RequestResult +=new ConnectionContext.Main((Result result)=>{

                if (result.ServerName == Helper.PartToServerName(Part.blobs, this.CustomServerAddr) || result.ServerName.Contains("amazonaws"))
                {

                    switch (result.Verbs)
                    {
                        case AcceptVerbs.GET:
                            {
                                if (result.ControllerName.Contains("/blobs/") && result.ControllerName.Contains(".xml"))
                                {
                                    this.GetInformationAboutFile_Response(result);
                                    return;
                                }
                                if (result.ControllerName.Contains("/blobs/") && result.ControllerName.Contains(".ext"))
                                {

                                    return;
                                }
                                if (result.ControllerName.Contains("amazonaws"))
                                {
                                    this.DownloadFile_Response(result);
                                    return;
                                }
                                break;
                            }
                        case AcceptVerbs.DELETE:
                            {
                                if (result.ControllerName.Contains("/blobs/") && result.ControllerName.Contains(".xml"))
                                {
                                    this.Deletefile_Response(result);
                                    return;
                                }
                                break;
                            }
                        case AcceptVerbs.PUT:
                            {
                                if (result.ControllerName.Contains("/blobs/") && result.ControllerName.Contains(".xml"))
                                {
                                    this.DeclaringFileUploaded_Response(result);
                                    return;
                                }
                                break;
                            }
                        case AcceptVerbs.POST:
                            {
                                if (result.ControllerName.Contains("/blobs"))
                                {
                                    this.CreateFile_Response(result);
                                    return;
                                }

                                if (result.ControllerName.Contains("/blobs/") && result.ControllerName.Contains("/getblobobjectbyid.xml"))
                                {
                                    this.GetFileByID_Response(result);
                                    return;
                                }
                                if (result.ControllerName.Contains("amazonaws"))
                                {
                                    this.UploadFile_Response(result);
                                    return;
                                }
                                break;
                            }
                    }
                }
            });
        }





        private void CreateFile_Response(Result result)
        {
            if (ContentServiceEvent != null) // если привязан обработчик
            {
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = ContentServiceCommand.CreateFile,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = ContentServiceCommand.CreateFile,
                            errorMessage = ex.Message
                        });
                    }
                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.CreateFile,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.CreateFile,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = ContentServiceCommand.CreateFile,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void CreateFile(string MIMEType, string Name, uint Multipart)
        {
            if(MIMEType == null || Name == null)
                return;

            this.Сontext.CurrentPart = Part.blobs;

            this.Сontext.Add("blob[blob_owner_id", this.OwnerId.ToString());
            this.Сontext.Add("blob[content_type]", MIMEType);
            this.Сontext.Add("blob[name]", Name);
            this.Сontext.Add("blob[multipart]", Multipart.ToString());

            this.Сontext.SendAsyncRequest("blobs.xml", AcceptVerbs.POST);
        }



        private void UploadFile_Response(Result result)
        {
            if (ContentServiceEvent != null) // если привязан обработчик
            {
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = ContentServiceCommand.CreateFile,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = ContentServiceCommand.CreateFile,
                            errorMessage = ex.Message
                        });
                    }
                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.CreateFile,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.CreateFile,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = ContentServiceCommand.CreateFile,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void UploadFile(string Param, byte[] Content)
        {
            this.Сontext.CurrentPart = Part.blobs;
            //this.Сontext.CurrentServerAdr = "qbprod.s3.amazonaws.com";

            this.Сontext.Add("key", "");
            this.Сontext.Add("acl", "");
            this.Сontext.Add("success_action_status", "");
            this.Сontext.Add("Filename", "");
            this.Сontext.Add("AWSAccessKeyId", "");
            this.Сontext.Add("Policy", "");
            this.Сontext.Add("Signature", "");
            this.Сontext.Add("Content-Type", "");
            this.Сontext.Add("file", "");

            this.Сontext.SendAsyncRequest("", AcceptVerbs.POST);
        }



        private void DeclaringFileUploaded_Response(Result result)
        {
            if (ContentServiceEvent != null) // если привязан обработчик
            {
                if (result.ResultStatus == Status.OK) // если всё хорошо и пришол контент
                {
                    try
                    {

                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = ContentServiceCommand.DeclaringFileUploaded,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex) // ошибка распарсивания
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = ContentServiceCommand.DeclaringFileUploaded,
                            errorMessage = ex.Message
                        });
                    }
                }
                // нет контента из за ошибок
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.DeclaringFileUploaded,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.DeclaringFileUploaded,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = ContentServiceCommand.DeclaringFileUploaded,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void DeclaringFileUploaded(uint FileSize, uint BlobId)
        {
            this.Сontext.CurrentPart = Part.blobs;

            this.Сontext.Add("blob[size]", FileSize.ToString());

            this.Сontext.SendAsyncRequest("blobs/" + BlobId.ToString() +"/complete.xml", AcceptVerbs.PUT);
        }


        private void GetInformationAboutFile_Response(Result result)
        {
            if (ContentServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = new Blob(result.Content),
                            t = typeof(Blob),
                            status = result.ResultStatus,
                            currentCommand = ContentServiceCommand.GetInformationAboutFile,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = ContentServiceCommand.GetInformationAboutFile,
                            errorMessage = ex.Message
                        });
                    }
                }
                
                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.GetInformationAboutFile,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.GetInformationAboutFile,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = ContentServiceCommand.GetInformationAboutFile,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void GetInformationAboutFile(uint id)
        {
            this.Сontext.CurrentPart = Part.blobs;
            this.Сontext.SendAsyncRequest("blobs/" + id.ToString() + ".xml", AcceptVerbs.GET);
        }


        private void Deletefile_Response(Result result)
        {
            if (ContentServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = new Blob(result.Content),
                            t = typeof(Blob),
                            status = result.ResultStatus,
                            currentCommand = ContentServiceCommand.Deletefile,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = ContentServiceCommand.Deletefile,
                            errorMessage = ex.Message
                        });
                    }
                }

                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.Deletefile,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.Deletefile,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = ContentServiceCommand.Deletefile,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void Deletefile(uint id)
        {
            this.Сontext.CurrentPart = Part.blobs;
            this.Сontext.SendAsyncRequest("blobs/" + id.ToString() + ".xml", AcceptVerbs.DELETE);
        }



        private void DownloadFile_Response(Result result)
        {
            if (ContentServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = result.ResultStatus,
                            currentCommand = ContentServiceCommand.DownloadFile,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = ContentServiceCommand.DownloadFile,
                            errorMessage = ex.Message
                        });
                    }
                }

                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.DownloadFile,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.DownloadFile,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = ContentServiceCommand.DownloadFile,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void DownloadFile(string key)
        {
            this.Сontext.CurrentPart = Part.blobs;
            this.Сontext.SendAsyncRequest("blobs/" + key + ".ext", AcceptVerbs.GET);
        }



        private void GetFileByID_Response(Result result)
        {
            if (ContentServiceEvent != null)
            {
                if (result.ResultStatus == Status.OK)
                {
                    try
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = new BlobObjectAccess(result.Content),
                            t = typeof(BlobObjectAccess),
                            status = result.ResultStatus,
                            currentCommand = ContentServiceCommand.GetFileByID,
                            errorMessage = result.ErrorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        this.ContentServiceEvent(new ContentServiceEventArgs
                        {
                            result = null,
                            t = null,
                            status = Status.ContentError,
                            currentCommand = ContentServiceCommand.GetFileByID,
                            errorMessage = ex.Message
                        });
                    }
                }

                else if (result.ResultStatus == Status.StreamError
                    || result.ResultStatus == Status.TimeoutError
                    || result.ResultStatus == Status.UnknownError
                    || result.ResultStatus == Status.NotFoundError
                    || result.ResultStatus == Status.AccessDenied
                    || result.ResultStatus == Status.NotAcceptable
                    || result.ResultStatus == Status.ConnectionError
                    || result.ResultStatus == Status.AuthenticationError)
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = null,
                        t = null,
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.GetFileByID,
                        errorMessage = result.ErrorMessage
                    });
                else if (result.ResultStatus == Status.ValidationError)
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = ValidateErrorElement.LoadErrorList(result.Content),
                        t = typeof(ValidateErrorElement[]),
                        status = result.ResultStatus,
                        currentCommand = ContentServiceCommand.GetFileByID,
                        errorMessage = result.ErrorMessage
                    });
                }
                else
                {
                    this.ContentServiceEvent(new ContentServiceEventArgs
                    {
                        result = result.Content,
                        t = null,
                        status = Status.UnknownError,
                        currentCommand = ContentServiceCommand.GetFileByID,
                        errorMessage = result.ErrorMessage
                    });
                }
            }
        }

        public void GetFileByID(uint id)
        {
            this.Сontext.CurrentPart = Part.blobs;
            this.Сontext.SendAsyncRequest("blobs/" + id.ToString() + "/getblobobjectbyid.xml", AcceptVerbs.POST);
        }


    
    
    }

}
