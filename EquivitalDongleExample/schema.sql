-- Table for sessions (links all data tables via round_id)

-- Table for rounds
CREATE TABLE rounds (
    id SERIAL PRIMARY KEY,
    uid TEXT NOT NULL,
    player_id TEXT NOT NULL,
    round_id TEXT NOT NULL,
    start_time DOUBLE PRECISION NOT NULL,
    end_time DOUBLE PRECISION,
    UNIQUE (player_id, uid)
);

-- Table for ECG data
CREATE TABLE ecg_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,   
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    round_id TEXT NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    lead_one_raw SMALLINT,
    lead_two_raw SMALLINT,
    sequence_number SMALLINT,
    lead_one_mv DOUBLE PRECISION,
    lead_two_mv DOUBLE PRECISION
    -- CONSTRAINT fk_round_ecg FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Table for Heart Rate data
CREATE TABLE heart_rate_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    round_id TEXT NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    hr_bpm DOUBLE PRECISION
    -- CONSTRAINT fk_round_hr FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Table for Accelerometer data
CREATE TABLE accelerometer_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    round_id TEXT NOT NULL,
    vertical_mg DOUBLE PRECISION,
    lateral_mg DOUBLE PRECISION,
    longitudinal_mg DOUBLE PRECISION,
    resultant_mg DOUBLE PRECISION,
    vertical_raw SMALLINT,
    lateral_raw SMALLINT,
    longitudinal_raw SMALLINT
    -- CONSTRAINT fk_round_accel FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Table for Respiration Rate data
CREATE TABLE respiration_rate_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,   
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    round_id TEXT NOT NULL,
    breaths_per_minute DOUBLE PRECISION
    -- CONSTRAINT fk_round_rr FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Table for Impedance Respiration data
CREATE TABLE impedance_respiration_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    round_id TEXT NOT NULL,
    impedance SMALLINT
    -- CONSTRAINT fk_round_ir FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Table for Skin Temperature data
CREATE TABLE skin_temperature_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    round_id TEXT NOT NULL,
    temperature_deg DOUBLE PRECISION
    -- CONSTRAINT fk_round_temp FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Table for GSR data
CREATE TABLE gsr_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    round_id TEXT NOT NULL,
    raw_adc_reading SMALLINT,
    micro_siemens_reading DOUBLE PRECISION
    -- CONSTRAINT fk_round_gsr FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);


CREATE TABLE trajectories (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    uid TEXT NOT NULL,
    round_id TEXT NOT NULL,
    round_hash TEXT NOT NULL,
    state TEXT NOT NULL,
    joint_action TEXT NOT NULL,
    reward DOUBLE PRECISION NOT NULL,
    time_left INTEGER NOT NULL,
    score DOUBLE PRECISION NOT NULL,
    time_elapsed DOUBLE PRECISION NOT NULL,
    cur_gameloop DOUBLE PRECISION NOT NULL,
    layout TEXT NOT NULL,
    layout_name TEXT NOT NULL,
    trial_id TEXT NOT NULL,
    player_0_id TEXT NOT NULL,
    player_1_id TEXT NOT NULL,
    player_0_is_human BOOLEAN NOT NULL,
    player_1_is_human BOOLEAN NOT NULL,
    collision BOOLEAN NOT NULL,
    num_collisions INTEGER NOT NULL,
    unix_timestamp DOUBLE PRECISION NOT NULL
    -- CONSTRAINT fk_round_trajectories FOREIGN KEY (player_0_id, uid) REFERENCES rounds (player_id, uid)
);

CREATE TABLE eeg_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,   
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    round_id TEXT NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    labels TEXT[] NOT NULL,
    eeg_data FLOAT[] NOT NULL
    -- CONSTRAINT fk_round_eeg FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Indexes for EEG data
CREATE INDEX idx_eeg_round_id ON eeg_data (round_id);
CREATE INDEX idx_eeg_event_time ON eeg_data (event_time);


CREATE TABLE metrics_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,   
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    round_id TEXT NOT NULL,
    player_id TEXT NOT NULL,
    uid TEXT NOT NULL,
    labels TEXT[] NOT NULL,
    metrics_data FLOAT[] NOT NULL --temporarily made TEXT from FLOAT
    -- CONSTRAINT fk_round_metrics FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);

-- Indexes for EEG Metrics data
CREATE INDEX idx_metrics_round_id ON metrics_data (round_id);
CREATE INDEX idx_metrics_event_time ON metrics_data (event_time);


CREATE TABLE records (
    id SERIAL PRIMARY KEY,
    uid INTEGER NOT NULL,               -- Assuming UID is auto-incrementing
    unix_timestamp DOUBLE PRECISION NOT NULL,     -- Timestamp representing Unix timestamp (will use TO_TIMESTAMP for conversion)
    round_id VARCHAR(255) NOT NULL,        -- Assuming round_id is a string-based identifier
    start DOUBLE PRECISION NOT NULL,              -- Timestamp for the start time
    stop DOUBLE PRECISION NOT NULL,               -- Timestamp for the stop time
    CONSTRAINT valid_stop_time CHECK (stop > start)  -- Optional constraint to ensure stop time is after start time
);

CREATE INDEX idx_records_id ON records (id);
CREATE INDEX idx_trajectories_round_id ON trajectories (round_id);


CREATE TABLE IF NOT EXISTS eye_tracker_data
(
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time double precision NOT NULL,
    unix_timestamp double precision NOT NULL,
    lsl_timestamp double precision NOT NULL,
    round_id text COLLATE pg_catalog."default" NOT NULL,
    player_id text COLLATE pg_catalog."default" NOT NULL,
    uid TEXT NOT NULL,
    eye_tracker_data json
    -- CONSTRAINT fk_round_eye FOREIGN KEY (player_id, uid) REFERENCES rounds (player_id, uid)
);
END;

-- Unique index for rounds
CREATE UNIQUE INDEX idx_rounds_id ON rounds (id);

-- Indexes for ECG data
CREATE INDEX idx_ecg_round_id ON ecg_data (round_id);
CREATE INDEX idx_ecg_event_time ON ecg_data (event_time);

-- Indexes for Heart Rate data
CREATE INDEX idx_heart_rate_round_id ON heart_rate_data (round_id);
CREATE INDEX idx_heart_rate_event_time ON heart_rate_data (event_time);

-- Indexes for Accelerometer data
CREATE INDEX idx_accel_round_id ON accelerometer_data (round_id);
CREATE INDEX idx_accel_event_time ON accelerometer_data (event_time);

-- Indexes for Respiration Rate data
CREATE INDEX idx_respiration_round_id ON respiration_rate_data (round_id);
CREATE INDEX idx_respiration_event_time ON respiration_rate_data (event_time);

-- Indexes for Impedance Respiration data
CREATE INDEX idx_impedance_round_id ON impedance_respiration_data (round_id);
CREATE INDEX idx_impedance_event_time ON impedance_respiration_data (event_time);

-- Indexes for Skin Temperature data
CREATE INDEX idx_skin_temp_round_id ON skin_temperature_data (round_id);
CREATE INDEX idx_skin_temp_event_time ON skin_temperature_data (event_time);

-- Indexes for GSR data
CREATE INDEX idx_gsr_round_id ON gsr_data (round_id);
CREATE INDEX idx_gsr_event_time ON gsr_data (event_time);


--Keyboard events
CREATE TABLE session (
    hash_key TEXT PRIMARY KEY
);

CREATE TABLE keyboard_events (
    id SERIAL PRIMARY KEY,
    hash_key TEXT REFERENCES session(hash_key),
    timestamp TIMESTAMP NOT NULL,
    key TEXT NOT NULL,
    event_type TEXT NOT NULL
);

CREATE INDEX idx_keyboard_events_hash_key ON keyboard_events(hash_key);
CREATE INDEX idx_keyboard_events_timestamp ON keyboard_events(timestamp);
