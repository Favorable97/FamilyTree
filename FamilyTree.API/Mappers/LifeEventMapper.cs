namespace FamilyTree.API.Mappers
{
    public static class LifeEventMapper
    {
        public static List<LifeEventDTO> Map(List<LifeEvent> events)
        {
            List<LifeEventDTO> list = [];

            foreach (var lifeEvent in events)
            {
                LifeEventDTO ev = new()
                {
                    Type = lifeEvent.Type.ToString(),
                    Date = lifeEvent.Date,
                    Desciption = lifeEvent.Description ?? ""
                };

                list.Add(ev);
            }

            return list;
        }
    }
}
