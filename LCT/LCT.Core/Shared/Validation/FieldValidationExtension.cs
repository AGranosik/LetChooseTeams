using LCT.Core.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Core.Shared.Validation
{
    public static class FieldValidationExtension
    {
        public static void CheckIfNullOrEmpty<TField>(TField field, string fieldName)
            where TField : class
        {
            if (field == null || field == default(TField))
                throw new FieldCannotBeEmptyException(fieldName);
        }

        public static void CheckFieldLength(string field, string fieldName, int minLength = 0, int maxLength = int.MaxValue)
        {
            var fieldLength = field.Length;
            if(fieldLength <= minLength || fieldLength >= maxLength)
                throw new InvalidFieldException(fieldName);
        }

    }
    
}
