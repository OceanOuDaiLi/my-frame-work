using Model;
using UnityEngine;

namespace GameEngine
{
    [System.Serializable]

    public class CharacterProperty : ISerializationCallbackReceiver
    {
        private Character _character;
        public UserCharacter UserCharacter { get; set; }

        public void SetPropertyData(UserCharacter _userCharacter)
        {
            UserCharacter = _userCharacter;
            _character.TransformSelf.position = (Vector2)UserCharacter.data[0];
        }


        public void OnEnable(Character _character)
        {
            this._character = _character;
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {

        }
    }
}
