using TXServer.Core;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents;

namespace TXServer.ECSSystem.Components.User
{
    [SerialVersionUID(1545809085571)]
    public class UserAvatarComponent : Component
    {
        public UserAvatarComponent(long avatarMarketId, Player player)
        {
            string configPath = (player.GetEntityById(avatarMarketId) ?? Avatars.GlobalItems.Tankist).TemplateAccessor
                .ConfigPath;
            Id = Config.GetComponent<AvatarItemComponent>(configPath).Id;
        }

        public string Id { get; set; }
    }
}
