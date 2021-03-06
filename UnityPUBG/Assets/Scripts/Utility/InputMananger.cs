// GENERATED AUTOMATICALLY FROM 'Assets/Settings/InputManager.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace UnityPUBG.Scripts.Utilities
{
    public class InputManager : IInputActionCollection
    {
        private InputActionAsset asset;
        public InputManager()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""57b98f3e-e431-4a22-8420-8b32e69dea91"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""dab3b302-0c9a-424b-9dac-2224417675dd"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""937d0869-2179-421c-9399-654a44779800"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3829a1b6-e351-417e-bd3e-36376a35c640"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3823a9ff-1c97-4e45-be0b-aa5b8b3bc35d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c32152fc-111f-4178-b187-c502730f5910"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f29b5306-d671-4479-8445-5edd000a3e8f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""CharacterSpawner"",
            ""id"": ""aa1cdb76-a3c6-4ce1-89be-96a6f39dbe20"",
            ""actions"": [
                {
                    ""name"": ""Spawn"",
                    ""type"": ""Button"",
                    ""id"": ""d1454e94-0ef3-45b0-9f8f-b37a85c5428d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""31586fd4-40c6-45d7-b038-840665d50a8f"",
                    ""path"": ""<Mouse>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Spawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""305175ae-d7ee-47a2-99ed-23c816961bd9"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Spawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc5f77ed-5528-43aa-b792-02fcfbe6f3de"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Spawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""defac7f4-6274-46c2-baf5-686be548c256"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Spawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5a381c8d-20ae-4143-b940-bc0a0e763440"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Spawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""936ea8e4-46ff-4b28-8e97-0fdee27bb9c1"",
                    ""path"": ""<Touchscreen>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Spawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
            // CharacterSpawner
            m_CharacterSpawner = asset.FindActionMap("CharacterSpawner", throwIfNotFound: true);
            m_CharacterSpawner_Spawn = m_CharacterSpawner.FindAction("Spawn", throwIfNotFound: true);
        }

        ~InputManager()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Movement;
        public struct PlayerActions
        {
            private InputManager m_Wrapper;
            public PlayerActions(InputManager wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Player_Movement;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Movement.started += instance.OnMovement;
                    Movement.performed += instance.OnMovement;
                    Movement.canceled += instance.OnMovement;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // CharacterSpawner
        private readonly InputActionMap m_CharacterSpawner;
        private ICharacterSpawnerActions m_CharacterSpawnerActionsCallbackInterface;
        private readonly InputAction m_CharacterSpawner_Spawn;
        public struct CharacterSpawnerActions
        {
            private InputManager m_Wrapper;
            public CharacterSpawnerActions(InputManager wrapper) { m_Wrapper = wrapper; }
            public InputAction @Spawn => m_Wrapper.m_CharacterSpawner_Spawn;
            public InputActionMap Get() { return m_Wrapper.m_CharacterSpawner; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CharacterSpawnerActions set) { return set.Get(); }
            public void SetCallbacks(ICharacterSpawnerActions instance)
            {
                if (m_Wrapper.m_CharacterSpawnerActionsCallbackInterface != null)
                {
                    Spawn.started -= m_Wrapper.m_CharacterSpawnerActionsCallbackInterface.OnSpawn;
                    Spawn.performed -= m_Wrapper.m_CharacterSpawnerActionsCallbackInterface.OnSpawn;
                    Spawn.canceled -= m_Wrapper.m_CharacterSpawnerActionsCallbackInterface.OnSpawn;
                }
                m_Wrapper.m_CharacterSpawnerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Spawn.started += instance.OnSpawn;
                    Spawn.performed += instance.OnSpawn;
                    Spawn.canceled += instance.OnSpawn;
                }
            }
        }
        public CharacterSpawnerActions @CharacterSpawner => new CharacterSpawnerActions(this);
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
                return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        private int m_TouchSchemeIndex = -1;
        public InputControlScheme TouchScheme
        {
            get
            {
                if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.FindControlSchemeIndex("Touch");
                return asset.controlSchemes[m_TouchSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnMovement(InputAction.CallbackContext context);
        }
        public interface ICharacterSpawnerActions
        {
            void OnSpawn(InputAction.CallbackContext context);
        }
    }
}
