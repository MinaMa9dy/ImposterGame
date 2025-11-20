using Imposter.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposter.Core.Domain.Entities
{
    public class SecretWord
    {
        [Key]
        public string Text { get; set; }
        public CategoryOptions Category { get; set; }
    }
}
