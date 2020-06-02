using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636413315444096863L)]
    public class TutorialGameplayChestUserItemTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Entity marketItem, Entity user) => ContainerUserItemTemplate.CreateEntity(new TutorialGameplayChestUserItemTemplate(), marketItem, user);
    }
}
