using Newtonsoft.Json;

namespace encompass_find_loans {
    public class SRLoanNumber {
        public string LoanNumber {get;set;}
    }

    public class Event {
        [JsonProperty(PropertyName = "audience")]
        public string Audience { get; set; }

        [JsonProperty(PropertyName = "origin")]
        public Origin Origin { get; set; }

        [JsonProperty(PropertyName = "defaultMessage")]
        public dynamic DefaultMessage { get; set; }

        [JsonProperty(PropertyName = "correlationId")]
        public string CorrelationId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "relationshipId")]
        public string RelationshipId { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public string Priority { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "recordedTimestamp")]
        public string RecordedTimeStamp { get; set; }
    }

    public class Origin {
        [JsonProperty(PropertyName = "systemName")]
        public string Audience { get; set; }

        [JsonProperty(PropertyName = "data")]
        public dynamic Data { get; set; }

        [JsonProperty(PropertyName = "dataSchemaName")]
        public string DataSchemaName { get; set; }

    }
}