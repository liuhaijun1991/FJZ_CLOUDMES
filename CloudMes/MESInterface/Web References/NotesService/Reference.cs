//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 原始程式碼已由 Microsoft.VSDesigner 自動產生，版本 4.0.30319.42000。
// 
#pragma warning disable 1591

namespace MESInterface.NotesService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="LotusNotesServiceSoap", Namespace="http://tempuri.org/")]
    public partial class LotusNotesService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback SendNotesMailOperationCompleted;
        
        private System.Threading.SendOrPostCallback testOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public LotusNotesService() {
            this.Url = global::MESInterface.Properties.Settings.Default.MESInterface_NotesService_LotusNotesService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event SendNotesMailCompletedEventHandler SendNotesMailCompleted;
        
        /// <remarks/>
        public event testCompletedEventHandler testCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendNotesMail", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string SendNotesMail(LotusMail Lmail) {
            object[] results = this.Invoke("SendNotesMail", new object[] {
                        Lmail});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SendNotesMailAsync(LotusMail Lmail) {
            this.SendNotesMailAsync(Lmail, null);
        }
        
        /// <remarks/>
        public void SendNotesMailAsync(LotusMail Lmail, object userState) {
            if ((this.SendNotesMailOperationCompleted == null)) {
                this.SendNotesMailOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSendNotesMailOperationCompleted);
            }
            this.InvokeAsync("SendNotesMail", new object[] {
                        Lmail}, this.SendNotesMailOperationCompleted, userState);
        }
        
        private void OnSendNotesMailOperationCompleted(object arg) {
            if ((this.SendNotesMailCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SendNotesMailCompleted(this, new SendNotesMailCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/test", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string test(string filename) {
            object[] results = this.Invoke("test", new object[] {
                        filename});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void testAsync(string filename) {
            this.testAsync(filename, null);
        }
        
        /// <remarks/>
        public void testAsync(string filename, object userState) {
            if ((this.testOperationCompleted == null)) {
                this.testOperationCompleted = new System.Threading.SendOrPostCallback(this.OntestOperationCompleted);
            }
            this.InvokeAsync("test", new object[] {
                        filename}, this.testOperationCompleted, userState);
        }
        
        private void OntestOperationCompleted(object arg) {
            if ((this.testCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.testCompleted(this, new testCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class LotusMail {
        
        private string[] mailToField;
        
        private string mailTitleField;
        
        private string mailAttachmentField;
        
        private string mailBodyField;
        
        private bool mailSaveField;
        
        /// <remarks/>
        public string[] MailTo {
            get {
                return this.mailToField;
            }
            set {
                this.mailToField = value;
            }
        }
        
        /// <remarks/>
        public string MailTitle {
            get {
                return this.mailTitleField;
            }
            set {
                this.mailTitleField = value;
            }
        }
        
        /// <remarks/>
        public string MailAttachment {
            get {
                return this.mailAttachmentField;
            }
            set {
                this.mailAttachmentField = value;
            }
        }
        
        /// <remarks/>
        public string MailBody {
            get {
                return this.mailBodyField;
            }
            set {
                this.mailBodyField = value;
            }
        }
        
        /// <remarks/>
        public bool MailSave {
            get {
                return this.mailSaveField;
            }
            set {
                this.mailSaveField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void SendNotesMailCompletedEventHandler(object sender, SendNotesMailCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SendNotesMailCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SendNotesMailCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void testCompletedEventHandler(object sender, testCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class testCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal testCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591