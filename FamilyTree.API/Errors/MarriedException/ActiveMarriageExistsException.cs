namespace FamilyTree.API.Errors
{
    public class ActiveMarriageExistsException() : DomainException("Нельзя зарегистрировать новый брак для одного из супругов, так как у него есть активный брак")
    {
        public override ErrorCode ErrorCode => ErrorCode.ActiveMarriageExists;
    }
}
