using LCT.Core.Shared.Exceptions;
using LCT.Domain.Common.Exceptions;

namespace LCT.Domain.Common.Validation
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
