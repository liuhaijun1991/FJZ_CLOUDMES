﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MESMailCenter.SMTPService {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", ConfigurationName="SMTPService.SmtpServiceSoap")]
    public interface SmtpServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/GeterrMsg", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        string GeterrMsg();
        
        // CODEGEN: 訊息 SendMailRequest 具有標頭，正在產生訊息合約
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/SendMail", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        MESMailCenter.SMTPService.SendMailResponse SendMail(MESMailCenter.SMTPService.SendMailRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/WMSendMail", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        bool WMSendMail(string mailto, string from, string cc, string subject, string msg);
        
        // CODEGEN: 訊息 SaveAttachmentRequest 具有標頭，正在產生訊息合約
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/SaveAttachment", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        MESMailCenter.SMTPService.SaveAttachmentResponse SaveAttachment(MESMailCenter.SMTPService.SaveAttachmentRequest request);
        
        // CODEGEN: 訊息 CleanAttachRequest 具有標頭，正在產生訊息合約
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/CleanAttach", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        MESMailCenter.SMTPService.CleanAttachResponse CleanAttach(MESMailCenter.SMTPService.CleanAttachRequest request);
        
        // CODEGEN: 訊息 SAPDownRequest 具有標頭，正在產生訊息合約
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/SAPDown", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        MESMailCenter.SMTPService.SAPDownResponse SAPDown(MESMailCenter.SMTPService.SAPDownRequest request);
        
        // CODEGEN: 訊息 SAPDown_toRequest 具有標頭，正在產生訊息合約
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/SAPDown_to", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        MESMailCenter.SMTPService.SAPDown_toResponse SAPDown_to(MESMailCenter.SMTPService.SAPDown_toRequest request);
        
        // CODEGEN: 訊息 NotesIDRequest 具有標頭，正在產生訊息合約
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SmtpService/SmtpService/NotesID", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        MESMailCenter.SMTPService.NotesIDResponse NotesID(MESMailCenter.SMTPService.NotesIDRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
    public partial class ContainsKey : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string containsKey1Field;
        
        private string userNameField;
        
        private string passwordField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ContainsKey", Order=0)]
        public string ContainsKey1 {
            get {
                return this.containsKey1Field;
            }
            set {
                this.containsKey1Field = value;
                this.RaisePropertyChanged("ContainsKey1");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string UserName {
            get {
                return this.userNameField;
            }
            set {
                this.userNameField = value;
                this.RaisePropertyChanged("UserName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("Password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
                this.RaisePropertyChanged("AnyAttr");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
    public partial class MailStruct : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string sAPtoField;
        
        private string mailtoField;
        
        private string ccField;
        
        private string bccField;
        
        private string fromField;
        
        private string subjectField;
        
        private string bodyField;
        
        private string filePathField;
        
        private string ifHZField;
        
        private MailFormat formatField;
        
        private MailPriority priorityField;
        
        private MailEncoding encodeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string SAPto {
            get {
                return this.sAPtoField;
            }
            set {
                this.sAPtoField = value;
                this.RaisePropertyChanged("SAPto");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string mailto {
            get {
                return this.mailtoField;
            }
            set {
                this.mailtoField = value;
                this.RaisePropertyChanged("mailto");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string cc {
            get {
                return this.ccField;
            }
            set {
                this.ccField = value;
                this.RaisePropertyChanged("cc");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string bcc {
            get {
                return this.bccField;
            }
            set {
                this.bccField = value;
                this.RaisePropertyChanged("bcc");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string from {
            get {
                return this.fromField;
            }
            set {
                this.fromField = value;
                this.RaisePropertyChanged("from");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string subject {
            get {
                return this.subjectField;
            }
            set {
                this.subjectField = value;
                this.RaisePropertyChanged("subject");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string body {
            get {
                return this.bodyField;
            }
            set {
                this.bodyField = value;
                this.RaisePropertyChanged("body");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public string FilePath {
            get {
                return this.filePathField;
            }
            set {
                this.filePathField = value;
                this.RaisePropertyChanged("FilePath");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string ifHZ {
            get {
                return this.ifHZField;
            }
            set {
                this.ifHZField = value;
                this.RaisePropertyChanged("ifHZ");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public MailFormat format {
            get {
                return this.formatField;
            }
            set {
                this.formatField = value;
                this.RaisePropertyChanged("format");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=10)]
        public MailPriority priority {
            get {
                return this.priorityField;
            }
            set {
                this.priorityField = value;
                this.RaisePropertyChanged("priority");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=11)]
        public MailEncoding encode {
            get {
                return this.encodeField;
            }
            set {
                this.encodeField = value;
                this.RaisePropertyChanged("encode");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
    public enum MailFormat {
        
        /// <remarks/>
        Text,
        
        /// <remarks/>
        Html,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
    public enum MailPriority {
        
        /// <remarks/>
        Normal,
        
        /// <remarks/>
        Low,
        
        /// <remarks/>
        High,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3062.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
    public enum MailEncoding {
        
        /// <remarks/>
        UUEncode,
        
        /// <remarks/>
        Base64,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SendMail", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SendMailRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public MESMailCenter.SMTPService.MailStruct obj;
        
        public SendMailRequest() {
        }
        
        public SendMailRequest(MESMailCenter.SMTPService.ContainsKey ContainsKey, MESMailCenter.SMTPService.MailStruct obj) {
            this.ContainsKey = ContainsKey;
            this.obj = obj;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SendMailResponse", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SendMailResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public bool SendMailResult;
        
        public SendMailResponse() {
        }
        
        public SendMailResponse(MESMailCenter.SMTPService.ContainsKey ContainsKey, bool SendMailResult) {
            this.ContainsKey = ContainsKey;
            this.SendMailResult = SendMailResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SaveAttachment", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SaveAttachmentRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public string name;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] Buffer;
        
        public SaveAttachmentRequest() {
        }
        
        public SaveAttachmentRequest(MESMailCenter.SMTPService.ContainsKey ContainsKey, string name, byte[] Buffer) {
            this.ContainsKey = ContainsKey;
            this.name = name;
            this.Buffer = Buffer;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SaveAttachmentResponse", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SaveAttachmentResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public bool SaveAttachmentResult;
        
        public SaveAttachmentResponse() {
        }
        
        public SaveAttachmentResponse(MESMailCenter.SMTPService.ContainsKey ContainsKey, bool SaveAttachmentResult) {
            this.ContainsKey = ContainsKey;
            this.SaveAttachmentResult = SaveAttachmentResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CleanAttach", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class CleanAttachRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        public CleanAttachRequest() {
        }
        
        public CleanAttachRequest(MESMailCenter.SMTPService.ContainsKey ContainsKey) {
            this.ContainsKey = ContainsKey;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CleanAttachResponse", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class CleanAttachResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public bool CleanAttachResult;
        
        public CleanAttachResponse() {
        }
        
        public CleanAttachResponse(MESMailCenter.SMTPService.ContainsKey ContainsKey, bool CleanAttachResult) {
            this.ContainsKey = ContainsKey;
            this.CleanAttachResult = CleanAttachResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SAPDown", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SAPDownRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public string SAP_to;
        
        public SAPDownRequest() {
        }
        
        public SAPDownRequest(MESMailCenter.SMTPService.ContainsKey ContainsKey, string SAP_to) {
            this.ContainsKey = ContainsKey;
            this.SAP_to = SAP_to;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SAPDownResponse", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SAPDownResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public bool SAPDownResult;
        
        public SAPDownResponse() {
        }
        
        public SAPDownResponse(MESMailCenter.SMTPService.ContainsKey ContainsKey, bool SAPDownResult) {
            this.ContainsKey = ContainsKey;
            this.SAPDownResult = SAPDownResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SAPDown_to", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SAPDown_toRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public string ToNo;
        
        public SAPDown_toRequest() {
        }
        
        public SAPDown_toRequest(MESMailCenter.SMTPService.ContainsKey ContainsKey, string ToNo) {
            this.ContainsKey = ContainsKey;
            this.ToNo = ToNo;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SAPDown_toResponse", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class SAPDown_toResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public System.Data.DataTable SAPDown_toResult;
        
        public SAPDown_toResponse() {
        }
        
        public SAPDown_toResponse(MESMailCenter.SMTPService.ContainsKey ContainsKey, System.Data.DataTable SAPDown_toResult) {
            this.ContainsKey = ContainsKey;
            this.SAPDown_toResult = SAPDown_toResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="NotesID", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class NotesIDRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public string LogonName;
        
        public NotesIDRequest() {
        }
        
        public NotesIDRequest(MESMailCenter.SMTPService.ContainsKey ContainsKey, string LogonName) {
            this.ContainsKey = ContainsKey;
            this.LogonName = LogonName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="NotesIDResponse", WrapperNamespace="http://tempuri.org/SmtpService/SmtpService", IsWrapped=true)]
    public partial class NotesIDResponse {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService")]
        public MESMailCenter.SMTPService.ContainsKey ContainsKey;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/SmtpService/SmtpService", Order=0)]
        public string NotesIDResult;
        
        public NotesIDResponse() {
        }
        
        public NotesIDResponse(MESMailCenter.SMTPService.ContainsKey ContainsKey, string NotesIDResult) {
            this.ContainsKey = ContainsKey;
            this.NotesIDResult = NotesIDResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SmtpServiceSoapChannel : MESMailCenter.SMTPService.SmtpServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SmtpServiceSoapClient : System.ServiceModel.ClientBase<MESMailCenter.SMTPService.SmtpServiceSoap>, MESMailCenter.SMTPService.SmtpServiceSoap {
        
        public SmtpServiceSoapClient() {
        }
        
        public SmtpServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SmtpServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SmtpServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SmtpServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string GeterrMsg() {
            return base.Channel.GeterrMsg();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MESMailCenter.SMTPService.SendMailResponse MESMailCenter.SMTPService.SmtpServiceSoap.SendMail(MESMailCenter.SMTPService.SendMailRequest request) {
            return base.Channel.SendMail(request);
        }
        
        public bool SendMail(ref MESMailCenter.SMTPService.ContainsKey ContainsKey, MESMailCenter.SMTPService.MailStruct obj) {
            MESMailCenter.SMTPService.SendMailRequest inValue = new MESMailCenter.SMTPService.SendMailRequest();
            inValue.ContainsKey = ContainsKey;
            inValue.obj = obj;
            MESMailCenter.SMTPService.SendMailResponse retVal = ((MESMailCenter.SMTPService.SmtpServiceSoap)(this)).SendMail(inValue);
            ContainsKey = retVal.ContainsKey;
            return retVal.SendMailResult;
        }
        
        public bool WMSendMail(string mailto, string from, string cc, string subject, string msg) {
            return base.Channel.WMSendMail(mailto, from, cc, subject, msg);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MESMailCenter.SMTPService.SaveAttachmentResponse MESMailCenter.SMTPService.SmtpServiceSoap.SaveAttachment(MESMailCenter.SMTPService.SaveAttachmentRequest request) {
            return base.Channel.SaveAttachment(request);
        }
        
        public bool SaveAttachment(ref MESMailCenter.SMTPService.ContainsKey ContainsKey, string name, byte[] Buffer) {
            MESMailCenter.SMTPService.SaveAttachmentRequest inValue = new MESMailCenter.SMTPService.SaveAttachmentRequest();
            inValue.ContainsKey = ContainsKey;
            inValue.name = name;
            inValue.Buffer = Buffer;
            MESMailCenter.SMTPService.SaveAttachmentResponse retVal = ((MESMailCenter.SMTPService.SmtpServiceSoap)(this)).SaveAttachment(inValue);
            ContainsKey = retVal.ContainsKey;
            return retVal.SaveAttachmentResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MESMailCenter.SMTPService.CleanAttachResponse MESMailCenter.SMTPService.SmtpServiceSoap.CleanAttach(MESMailCenter.SMTPService.CleanAttachRequest request) {
            return base.Channel.CleanAttach(request);
        }
        
        public bool CleanAttach(ref MESMailCenter.SMTPService.ContainsKey ContainsKey) {
            MESMailCenter.SMTPService.CleanAttachRequest inValue = new MESMailCenter.SMTPService.CleanAttachRequest();
            inValue.ContainsKey = ContainsKey;
            MESMailCenter.SMTPService.CleanAttachResponse retVal = ((MESMailCenter.SMTPService.SmtpServiceSoap)(this)).CleanAttach(inValue);
            ContainsKey = retVal.ContainsKey;
            return retVal.CleanAttachResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MESMailCenter.SMTPService.SAPDownResponse MESMailCenter.SMTPService.SmtpServiceSoap.SAPDown(MESMailCenter.SMTPService.SAPDownRequest request) {
            return base.Channel.SAPDown(request);
        }
        
        public bool SAPDown(ref MESMailCenter.SMTPService.ContainsKey ContainsKey, string SAP_to) {
            MESMailCenter.SMTPService.SAPDownRequest inValue = new MESMailCenter.SMTPService.SAPDownRequest();
            inValue.ContainsKey = ContainsKey;
            inValue.SAP_to = SAP_to;
            MESMailCenter.SMTPService.SAPDownResponse retVal = ((MESMailCenter.SMTPService.SmtpServiceSoap)(this)).SAPDown(inValue);
            ContainsKey = retVal.ContainsKey;
            return retVal.SAPDownResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MESMailCenter.SMTPService.SAPDown_toResponse MESMailCenter.SMTPService.SmtpServiceSoap.SAPDown_to(MESMailCenter.SMTPService.SAPDown_toRequest request) {
            return base.Channel.SAPDown_to(request);
        }
        
        public System.Data.DataTable SAPDown_to(ref MESMailCenter.SMTPService.ContainsKey ContainsKey, string ToNo) {
            MESMailCenter.SMTPService.SAPDown_toRequest inValue = new MESMailCenter.SMTPService.SAPDown_toRequest();
            inValue.ContainsKey = ContainsKey;
            inValue.ToNo = ToNo;
            MESMailCenter.SMTPService.SAPDown_toResponse retVal = ((MESMailCenter.SMTPService.SmtpServiceSoap)(this)).SAPDown_to(inValue);
            ContainsKey = retVal.ContainsKey;
            return retVal.SAPDown_toResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        MESMailCenter.SMTPService.NotesIDResponse MESMailCenter.SMTPService.SmtpServiceSoap.NotesID(MESMailCenter.SMTPService.NotesIDRequest request) {
            return base.Channel.NotesID(request);
        }
        
        public string NotesID(ref MESMailCenter.SMTPService.ContainsKey ContainsKey, string LogonName) {
            MESMailCenter.SMTPService.NotesIDRequest inValue = new MESMailCenter.SMTPService.NotesIDRequest();
            inValue.ContainsKey = ContainsKey;
            inValue.LogonName = LogonName;
            MESMailCenter.SMTPService.NotesIDResponse retVal = ((MESMailCenter.SMTPService.SmtpServiceSoap)(this)).NotesID(inValue);
            ContainsKey = retVal.ContainsKey;
            return retVal.NotesIDResult;
        }
    }
}
