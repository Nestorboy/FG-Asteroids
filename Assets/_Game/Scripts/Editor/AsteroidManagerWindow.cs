using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class AsteroidManagerWindow : EditorWindow
{
    private VisualElement _root;
    [SerializeField] private bool _initialized;
    
    [SerializeField] private Asteroids.AsteroidSpawner _spawner;
    private Box _spawnerContainer;
    private ObjectField _spawnerField;
    private InspectorElement _spawnerInspector;

    [SerializeField] private Ship.Engine _engine;
    private Box _engineContainer;
    private ObjectField _engineField;
    private InspectorElement _engineInspector;
    
    private Asteroids.AsteroidSpawner spawner
    {
        get => _spawner;
        set
        {
            _spawner = value;
            
            _spawnerContainer.Clear();

            _spawnerInspector = new InspectorElement(value);
            if (value == null)
            {
                _spawnerContainer.Add(new Label("Please select an AsteroidSpawner to modify."));
            }
            else
            {
                _spawnerContainer.Add(_spawnerInspector);
            }
        }
    }
    
    private Ship.Engine engine
    {
        get => _engine;
        set
        {
            _engine = value;
            
            _engineContainer.Clear();

            _engineInspector = new InspectorElement(value);
            if (value == null)
            {
                _engineContainer.Add(new Label("Please select an Engine to modify."));
            }
            else
            {
                _engineContainer.Add(_engineInspector);
            }
        }
    }

    [MenuItem("Window/Nessie/Asteroid Manager")]
    public static void OpenWindow()
    {
        var window = GetWindow<AsteroidManagerWindow>("Asteroid Manager");
        window.minSize = new Vector2(350, 350);
    }

    public void OnEnable()
    {
        Undo.undoRedoPerformed += UndoCallback;
        
        if (_initialized)
            return;
        
        _spawner = FindObjectOfType<Asteroids.AsteroidSpawner>();
        _engine = FindObjectOfType<Ship.Engine>();

        _initialized = true;
    }

    public void CreateGUI()
    {
        _root = rootVisualElement;
        
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Game/Scripts/Editor/CustomEditorWindow.uxml");
        VisualElement fromUXML = visualTree.Instantiate();
        _root.Add(fromUXML);
        
        _spawnerContainer = new Box();
        _spawnerField = fromUXML.Q<ObjectField>("SpawnerField");
        _spawnerField.parent.Add(_spawnerContainer);
        _spawnerField.value = _spawner;
        spawner = _spawner;
        _spawnerField.RegisterValueChangedCallback(SpawnerChanged);
        
        _engineContainer = new Box();
        _engineField = fromUXML.Q<ObjectField>("EngineField");
        _engineField.parent.Add(_engineContainer);
        _engineField.value = _engine;
        engine = _engine;
        _engineField.RegisterValueChangedCallback(ShipChanged);
    }

    private void UndoCallback()
    {
        _spawnerField.value = spawner;
        _engineField.value = engine;
    }
    
    private void SpawnerChanged(ChangeEvent<Object> evt)
    {
        Undo.RecordObject(this, "Changed Spawner");
        spawner = (Asteroids.AsteroidSpawner)evt.newValue;
    }

    private void ShipChanged(ChangeEvent<Object> evt)
    {
        Undo.RecordObject(this, "Changed Engine");
        engine = (Ship.Engine)evt.newValue;
    }
}