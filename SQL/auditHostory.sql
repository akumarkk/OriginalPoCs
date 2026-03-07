Create Database craftDb;

use craftDb;
CREATE TABLE Aircraft (
    AircraftID INT PRIMARY KEY,
    TailNumber NVARCHAR(20),
    Model NVARCHAR(50),
    -- These columns are required for temporal tracking
    SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN,
    SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime)
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.AircraftHistory));

-- Insert a record
INSERT INTO Aircraft (AircraftID, TailNumber, Model) 
VALUES (101, 'N123AB', 'Boeing 737');

-- Wait a second (to ensure time-based separation)
WAITFOR DELAY '00:00:01';

UPDATE Aircraft 
SET Model = 'Boeing 737-800' 
WHERE AircraftID = 101;


DELETE FROM Aircraft WHERE AircraftID = 101;

-- 1. Check current table (should be empty)
SELECT * FROM Aircraft;

-- 2. Check history table (should have 2 rows: one for the update, one for the delete)
SELECT *, SysStartTime, SysEndTime 
FROM dbo.AircraftHistory 
WHERE AircraftID = 101;

SELECT * FROM Aircraft 
FOR SYSTEM_TIME AS OF '2026-03-07 04:07:34.4851687'; -- Adjust time accordingly