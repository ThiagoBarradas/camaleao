using Flunt.Notifications;
using Newtonsoft.Json;
using System.Linq;

namespace Camaleao.Core.Entities
{
    public class Variable: Notifiable {

        public Variable() {
            this.Name = string.Empty;
            this.Type = string.Empty;
        }

        public string Name { get; set; }
        public dynamic Initialize { get; set; }
        public bool Builded { get; set; }
        private string _value;
        public string Value {
            get {
                if (string.IsNullOrEmpty(_value) && Initialize != null)
                    _value = Initialize.ToString();

                return _value;
            }
            set {
                _value = value;
            }
        }

        public string Type { get; set; }

        public void BuildVariable()
        {
            if (!this.Builded)
            {
                if (Initialize == null)
                {
                    Value = GetTypeValue();
                }
                else
                {
                    Value = MapperType();
                }
                Initialize = Value;
                this.Builded = true;
            }
        }

        private string MapperType()
        {
            switch (Initialize?.GetType()?.FullName)
            {
                case "System.Int32":
                case "System.Int64":
                    {
                        Type = "integer";
                        return $"{Initialize}";
                    }
                case "Newtonsoft.Json.Linq.JArray":
                case "Newtonsoft.Json.Linq.JObject":
                    {
                        Type = "object";
                        return $"{JsonConvert.SerializeObject(Initialize)}";
                    }
                case "System.Double":
                    {
                        Type = "double";
                        return $"{Initialize.ToString().Replace(',', '.')}";
                    }
                case "System.Boolean":
                    {
                        Type = "bool";
                        return $"{Initialize.ToString().ToLower()}";
                    }
                default:
                    {
                        Type = "text";
                        return $"'{Initialize}'";
                    }
            }
        }

        private string GetTypeValue()
        {
            switch (Type)
            {
                case "text":
                    return "''";
                case "integer":
                case "double":
                    return "0";
                case "bool":
                    return "true";
                case "array":
                    return "[]";
                default:
                    return "''";
            }
        }

        public bool IsValid() {

            if (string.IsNullOrWhiteSpace(Name)) {
                AddNotification("Variable", "[context.Variables] name is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.Type)) {
                AddNotification("Variable", $"[context.Variables] type is required in { this.Name}.");
                return false;
            }

            if (!Enuns.VariableTypeEnum.GetValues().Contains(this.Type.ToLower())) {
                AddNotification("Variable", $"[context.Variables] type is required in { this.Name}.");
                return false;
            }
            return true;
        }
    }
}
