-- Create WorkoutTypes table
CREATE TABLE WorkoutTypes (
    WorkoutTypeID INT PRIMARY KEY IDENTITY(1,1),  -- Auto-incrementing primary key
    ExerciseName NVARCHAR(100) NOT NULL           -- Name of the exercise
);

-- Create WorkoutSessions table
CREATE TABLE WorkoutSessions (
    SessionID INT PRIMARY KEY IDENTITY(1,1),      -- Auto-incrementing primary key
    SessionDate DATE NOT NULL                     -- Date of the workout session
);

-- Create Workouts table
CREATE TABLE Workouts (
    WorkoutID INT PRIMARY KEY IDENTITY(1,1),      -- Auto-incrementing primary key
    WorkoutTypeID INT NOT NULL,                   -- Foreign key to WorkoutTypes
    SessionID INT NOT NULL,                       -- Foreign key to WorkoutSessions
    PerceivedExertion TINYINT CHECK (PerceivedExertion BETWEEN 1 AND 4),  -- Exertion level (1-4)
    Repetitions INT NOT NULL,                     -- Number of repetitions
    Weight DECIMAL(5,2) NOT NULL,                 -- Weight used (e.g., in kg or lbs)
    
    FOREIGN KEY (WorkoutTypeID) REFERENCES WorkoutTypes(WorkoutTypeID),
    FOREIGN KEY (SessionID) REFERENCES WorkoutSessions(SessionID)
);


INSERT INTO WorkoutTypes(ExerciseName) Values('Biceps Curl')


-- Create PerceivedExertionLevels table
CREATE TABLE PerceivedExertionLevels (
    ExertionLevelID TINYINT PRIMARY KEY,       -- Primary key for exertion level (1-4)
    Description NVARCHAR(50) NOT NULL          -- Description of the exertion level
);

-- Populate PerceivedExertionLevels with initial data
INSERT INTO PerceivedExertionLevels (ExertionLevelID, Description)
VALUES 
    (1, 'Low'),
    (2, 'Medium'),
    (3, 'High'),
    (4, 'Failure');