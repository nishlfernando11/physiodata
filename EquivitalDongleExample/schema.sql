-- Table for sessions (to link all data tables via session_id)
CREATE TABLE sessions (
    session_id TEXT PRIMARY KEY,         -- Unique identifier for the session
    player_name TEXT,                    -- Player or user name
    start_time TIMESTAMP NOT NULL,       -- Session start time
    end_time TIMESTAMP                   -- Session end time
);

-- Table for ECG data
CREATE TABLE ecg_data (
    id SERIAL PRIMARY KEY,               -- Unique identifier
    created_at TIMESTAMP NOT NULL DEFAULT NOW(), -- Record creation time
    session_time TIMESTAMP NOT NULL,    -- Timestamp for ECG datapoint
    session_id TEXT NOT NULL,            -- Session ID (foreign key)
    lead_one_raw SMALLINT,               -- Raw Lead One data
    lead_two_raw SMALLINT,               -- Raw Lead Two data
    sequence_number SMALLINT,            -- Sequence number
    lead_one_mv DOUBLE PRECISION,        -- Voltage for Lead One (mV)
    lead_two_mv DOUBLE PRECISION,        -- Voltage for Lead Two (mV)
    CONSTRAINT fk_session_ecg FOREIGN KEY (session_id) REFERENCES sessions (session_id)
);

-- Table for Heart Rate data
CREATE TABLE heart_rate_data (
    id SERIAL PRIMARY KEY,               -- Unique identifier
    created_at TIMESTAMP NOT NULL DEFAULT NOW(), -- Record creation time
    session_time TIMESTAMP NOT NULL,     -- Timestamp for the heart rate datapoint
    session_id TEXT NOT NULL,            -- Session ID (foreign key)
    hr_bpm DOUBLE PRECISION,             -- Heart rate in beats per minute
    CONSTRAINT fk_session_hr FOREIGN KEY (session_id) REFERENCES sessions (session_id)
);

-- Table for Accelerometer data
CREATE TABLE accelerometer_data (
    id SERIAL PRIMARY KEY,               -- Unique identifier
    created_at TIMESTAMP NOT NULL DEFAULT NOW(), -- Record creation time
    session_time TIMESTAMP NOT NULL,     -- Timestamp for accelerometer datapoint
    session_id TEXT NOT NULL,            -- Session ID (foreign key)
    vertical_mg DOUBLE PRECISION,        -- Vertical acceleration in mG
    lateral_mg DOUBLE PRECISION,         -- Lateral acceleration in mG
    longitudinal_mg DOUBLE PRECISION,    -- Longitudinal acceleration in mG
    resultant_mg DOUBLE PRECISION,       -- Resultant acceleration in mG
    vertical_raw SMALLINT,               -- Raw vertical data
    lateral_raw SMALLINT,                -- Raw lateral data
    longitudinal_raw SMALLINT,           -- Raw longitudinal data
    CONSTRAINT fk_session_accel FOREIGN KEY (session_id) REFERENCES sessions (session_id)
);

-- Table for Respiration Rate data
CREATE TABLE respiration_rate_data (
    id SERIAL PRIMARY KEY,               -- Unique identifier
    created_at TIMESTAMP NOT NULL DEFAULT NOW(), -- Record creation time
    session_time TIMESTAMP NOT NULL,     -- Timestamp for respiration datapoint
    session_id TEXT NOT NULL,            -- Session ID (foreign key)
    breaths_per_minute DOUBLE PRECISION, -- Respiration rate (breaths per minute)
    CONSTRAINT fk_session_rr FOREIGN KEY (session_id) REFERENCES sessions (session_id)
);

-- Table for Impedance Respiration data
CREATE TABLE impedance_respiration_data (
    id SERIAL PRIMARY KEY,               -- Unique identifier
    created_at TIMESTAMP NOT NULL DEFAULT NOW(), -- Record creation time
    session_time TIMESTAMP NOT NULL,     -- Timestamp for impedance datapoint
    session_id TEXT NOT NULL,            -- Session ID (foreign key)
    impedance SMALLINT,                  -- Impedance value
    CONSTRAINT fk_session_ir FOREIGN KEY (session_id) REFERENCES sessions (session_id)
);

-- Table for Skin Temperature data
CREATE TABLE skin_temperature_data (
    id SERIAL PRIMARY KEY,               -- Unique identifier
    created_at TIMESTAMP NOT NULL DEFAULT NOW(), -- Record creation time
    session_time TIMESTAMP NOT NULL,     -- Timestamp for temperature datapoint
    session_id TEXT NOT NULL,            -- Session ID (foreign key)
    temperature_deg DOUBLE PRECISION,    -- Skin temperature in degrees
    CONSTRAINT fk_session_temp FOREIGN KEY (session_id) REFERENCES sessions (session_id)
);

-- Table for GSR data
CREATE TABLE gsr_data (
    id SERIAL PRIMARY KEY,               -- Unique identifier
    created_at TIMESTAMP NOT NULL DEFAULT NOW(), -- Record creation time
    session_time TIMESTAMP NOT NULL,          -- Timestamp for GSR datapoint
    session_id TEXT NOT NULL,                 -- Session ID (foreign key)
    raw_adc_reading SMALLINT,                 -- Raw ADC reading data
    micro_siemens_reading DOUBLE PRECISION,   -- Gets the reading in 100th of microSiemens (i.e. 10-8 Siemens)
    CONSTRAINT fk_session_gsr FOREIGN KEY (session_id) REFERENCES sessions (session_id)
);

-- Indexes

CREATE UNIQUE INDEX idx_session_id ON sessions (session_id);

CREATE INDEX idx_ecg_session_id ON ecg_data (session_id);
CREATE INDEX idx_ecg_session_time ON ecg_data (session_time);

CREATE INDEX idx_heart_rate_session_id ON heart_rate_data (session_id);
CREATE INDEX idx_heart_rate_session_time ON heart_rate_data (session_time);

CREATE INDEX idx_accel_session_id ON accelerometer_data (session_id);
CREATE INDEX idx_accel_session_time ON accelerometer_data (session_time);

CREATE INDEX idx_respiration_session_id ON respiration_rate_data (session_id);
CREATE INDEX idx_respiration_session_time ON respiration_rate_data (session_time);

CREATE INDEX idx_impedance_session_id ON impedance_respiration_data (session_id);
CREATE INDEX idx_impedance_session_time ON impedance_respiration_data (session_time);

CREATE INDEX idx_skin_temp_session_id ON skin_temperature_data (session_id);
CREATE INDEX idx_skin_temp_session_time ON skin_temperature_data (session_time);

CREATE INDEX idx_gsr_session_id ON gsr_data (session_id);
CREATE INDEX idx_gsr_session_time ON gsr_data (session_time);
