using UnityEngine;
using Newtonsoft.Json;
public class BrickPattern : MonoBehaviour
{
    [SerializeField]
    private TextAsset _jsonLevelPatterns;
    [SerializeField]
    public Brick _brickPrefab;
    private PatternCollection _levelPatterns;
    private PatternData _currentPattern;
    private int _levelSectionIndex;
    private Vector3 _originalPosition;
    public Vector3 _cubePos;

    private bool _levelOver;
    private bool _levelTransition;

    private BrickObjectPool _brickObjectPool;
    void Awake()
    {
        _brickObjectPool = GetComponent<BrickObjectPool>();

        _originalPosition = new Vector3(0f, 0.3f, -1.5f);
        _cubePos = transform.position;
        _levelTransition = false;
        _levelOver = false;

        /**
        Load level is just called on start, LoadLevel being public allows the flexibility of 
        another script to do something like load a level from an array of levels, stored somewhere else
        */
        LoadLevel();
    }

    private void Update()
    {
        // Check if the level is over and reset the flag
        if(_levelOver)
        {
            _levelOver = false;
        }

        // Check if there is a level transition and reset the flag
        if(_levelTransition)
        {
            _levelTransition = false;
        }

        // If there are no bricks under the game object as children, then do the following
        if(transform.childCount > 1) return;
        
        // Set level transition flag to true
        _levelTransition = true;

        // Check if all sections of the level have been traversed
        if(_levelSectionIndex >= _levelPatterns.sections.Length)
        {
            // Call the function to handle level completion
            LevelOver();

            return;
        }

        // Retrieve the current level pattern and increment the index to move to the next section

        _currentPattern = _levelPatterns.sections[_levelSectionIndex];
        _levelSectionIndex++;

        // Check if the current pattern is valid
        if(!ValidatePattern(_currentPattern))
        {
            return;
        };

        // If pattern is valid, spawn the pattern
        SpawnPattern(_currentPattern);
    }


    private void SpawnPattern(PatternData patternData)
    {
        // Variables to track the largest row and column lengths
        int largestRowLength = 0;
        int largestColLength = 0;

        // Set the initial position for spawning the pattern
        Vector3 patternPos = _originalPosition;
        transform.position = patternPos;
        
        // Variable to hold the position of each cube within the pattern
        Vector3 cubePos = transform.position;

        // Loop through each pattern in the pattern data
        foreach (PatternArray patternArray in patternData.pattern)
        {
            // Update the largest column length if the current array is longer
            if (patternArray.array.Length > largestColLength)
            {
                largestColLength = patternArray.array.Length;
            }
            
            // Loop through each row in the current 2D array
            for (int i = 0; i < patternArray.array.Length; i++)
            {
                // Update the largest row length if the current row is longer
                if (patternArray.array[i].Length > largestRowLength)
                {
                    largestRowLength = patternArray.array[i].Length;
                }

                // Loop through each element in the current row
                for (int j = 0; j < patternArray.array[i].Length; j++)
                {
                    // Calculate the position of the current cube within the pattern
                    cubePos.x = transform.position.x + 0.15f + (j / 3.33f);
                    cubePos.y = transform.position.y - 0.15f - (i / 3.33f);

                    // Instantiate a brick when the array value is 1 
                    // This is set up in a way that you could have different types of bricks depending on the value of array[i][j]
                    if (patternArray.array[i][j] == 1)
                    {
                        //Instantiate(_brickPrefab, cubePos, transform.rotation, transform);
                        _brickObjectPool.brickObjectPool.Get().gameObject.transform.position = cubePos;
                    }
                }            
            }
            
            // Move the pattern position forward so it's in front of the last 2D array
            patternPos.z += 0.305f;
            transform.position = patternPos;
        }

        // Adjust the final pattern position to center it properly within the level
        patternPos.x -= 0.15f * largestRowLength;
        patternPos.y += 0.3f * largestColLength;
        transform.position = patternPos;
    }


    private bool ValidatePattern(PatternData patternData)
    {
        // Define expected dimensions for the pattern data
        int expectedRowNum = 5;
        int expectedColNum = 10;
        int expectedArrayNum = 5;

        // Check if the number of 2D arrays in the pattern data exceeds the expected number
        if (patternData.pattern.Length > expectedArrayNum)
        {
            Debug.LogError("Current pattern: " + " Number of 2D arrays: " + patternData.pattern.Length + ". Expected: " + expectedRowNum);
            return false;
        }

        // Iterate through each 2D array in the pattern data
        foreach (PatternArray patternArray in patternData.pattern)
        {
            // Check if the number of rows in the current 2D array exceeds the expected number
            if (patternArray.array.Length > expectedRowNum)
            {
                Debug.LogError("Current pattern: " + " Number of rows: " + patternArray.array.Length + ". Expected: " + expectedRowNum);
                return false;
            }
            else
            {
                // Iterate through each row in the current 2D array
                for (int i = 0; i < patternArray.array.Length; i++)
                {
                    // Check if the number of elements in the current row exceeds the expected number
                    if (patternArray.array[i].Length > expectedColNum)
                    {
                        Debug.LogError("Current pattern: " + "Number of elements in row " + i + ": " + patternArray.array[i].Length + ". Expected: " + expectedColNum);
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private void LevelOver()
    {
        _levelOver = true;
  
        _levelSectionIndex = 0; // Currently ending the level currently restarts the current level because there isn't a system in place to swap out the json file
    }

    public bool IsLevelOver()
    {
        return _levelOver;
    }

    public bool IsLevelTransition()
    {
        return _levelTransition;
    }

    public void LoadLevel() // Load level is public because it may be used by accessed outside of this script in the future
    {
        if(_jsonLevelPatterns == null)
        {
            Debug.LogError("No JSON file");
            return;
        }

        

        _levelSectionIndex = 0;
        _levelPatterns = JsonConvert.DeserializeObject<PatternCollection>(_jsonLevelPatterns.text);
    }

    public void RestartLevel() // Restart level, resets the the section index back to 0 and destroys all of the bricks
    {
        _levelSectionIndex = 0;
        foreach(Brick brick in GetComponentsInChildren<Brick>())
        {
            _brickObjectPool.brickObjectPool.Release(brick);
        }
        {
            // if(transform.CompareTag("Brick"))
            // {
            //     _brickObjectPool.brickObjectPool;
            // }
        }
    }
}

// These classes are used to format the json file's data so that it's easier to interpret and work with the data
[System.Serializable]
public class PatternCollection
{
    public PatternData[] sections; // Each json file is a list of "sections"
}

[System.Serializable]
public class PatternData
{
    public PatternArray[] pattern; // Each section has a pattern, which is an array of 2D arrays
}

[System.Serializable]
public class PatternArray
{
    public int[][] array; // Each 2D array contains ints, 1's indicate bricks, 0's indicate empty spaces. Allows flexibility for different types of bricks
}
