// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace AdminClient.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class CreateMaterialDto
    {
        /// <summary>
        /// Initializes a new instance of the CreateMaterialDto class.
        /// </summary>
        public CreateMaterialDto() { }

        /// <summary>
        /// Initializes a new instance of the CreateMaterialDto class.
        /// </summary>
        public CreateMaterialDto(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Description == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Description");
            }
        }
    }
}
