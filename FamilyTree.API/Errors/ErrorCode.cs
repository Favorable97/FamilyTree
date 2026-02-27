namespace FamilyTree.API.Errors
{
    public enum ErrorCode
    {
        ValidationFailed,

        PersonNotFound,
        PersonAlreadyExists,
        InvalidParent,
        ParentNotFound,
        PersonHasChildren,
        FamilyTreeCycle,

        MarriageNotFound,
        ActiveMarriageExists,
        InvalidMarriageData,

        FatalError
    }
}
