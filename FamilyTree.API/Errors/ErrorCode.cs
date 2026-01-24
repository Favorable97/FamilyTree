namespace FamilyTree.API.Errors
{
    public enum ErrorCode
    {
        PersonNotFound,
        PersonAlreadyExists,
        InvalidParent,
        ParentNotFound,
        PersonHasChildren,
        FamilyTreeCycle,
        FatalError
    }
}
