﻿namespace OAuth20.Server.Validations
{
    public class BaseValidationResponse
    {
        public bool Succeeded { get; set; }
        public string Error { get; set; } = string.Empty;
        public string ErrorDescription { get; set; }
        public bool HasError => !string.IsNullOrEmpty(Error);
    }
}
