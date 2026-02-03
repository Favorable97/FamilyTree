using FamilyTree.API.Errors;

namespace FamilyTree.API.Mappers
{
    public static class ErrorCodeHttpMapper
    {
        public static int MapToStatusCode(ErrorCode code) =>
            code switch
            {
                ErrorCode.PersonNotFound => StatusCodes.Status404NotFound,
                ErrorCode.ParentNotFound => StatusCodes.Status404NotFound,

                ErrorCode.PersonAlreadyExists => StatusCodes.Status409Conflict,
                ErrorCode.PersonHasChildren => StatusCodes.Status409Conflict,

                ErrorCode.InvalidParent => StatusCodes.Status422UnprocessableEntity,

                _ => StatusCodes.Status404NotFound
            };
    }
}
