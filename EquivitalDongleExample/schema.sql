-- Table for sessions (links all data tables via round_id)

-- Table for rounds
CREATE TABLE rounds (
    id SERIAL  PRIMARY KEY,
    player_id TEXT NOT NULL,
    round_id TEXT NOT NULL,
    start_time DOUBLE PRECISION NOT NULL,
    end_time DOUBLE PRECISION
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
    lead_one_raw SMALLINT,
    lead_two_raw SMALLINT,
    sequence_number SMALLINT,
    lead_one_mv DOUBLE PRECISION,
    lead_two_mv DOUBLE PRECISION,
    CONSTRAINT fk_round_ecg FOREIGN KEY (round_id) REFERENCES rounds (round_id)
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
    hr_bpm DOUBLE PRECISION,
    CONSTRAINT fk_round_hr FOREIGN KEY (round_id) REFERENCES rounds (round_id)
);

-- Table for Accelerometer data
CREATE TABLE accelerometer_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    round_id TEXT NOT NULL,
    vertical_mg DOUBLE PRECISION,
    lateral_mg DOUBLE PRECISION,
    longitudinal_mg DOUBLE PRECISION,
    resultant_mg DOUBLE PRECISION,
    vertical_raw SMALLINT,
    lateral_raw SMALLINT,
    longitudinal_raw SMALLINT,
    CONSTRAINT fk_round_accel FOREIGN KEY (round_id) REFERENCES rounds (round_id)
);

-- Table for Respiration Rate data
CREATE TABLE respiration_rate_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,   
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    player_id TEXT NOT NULL,
    round_id TEXT NOT NULL,
    breaths_per_minute DOUBLE PRECISION,
    CONSTRAINT fk_round_rr FOREIGN KEY (round_id) REFERENCES rounds (round_id)
);

-- Table for Impedance Respiration data
CREATE TABLE impedance_respiration_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    round_id TEXT NOT NULL,
    impedance SMALLINT,
    CONSTRAINT fk_round_ir FOREIGN KEY (round_id) REFERENCES rounds (round_id)
);

-- Table for Skin Temperature data
CREATE TABLE skin_temperature_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    round_id TEXT NOT NULL,
    temperature_deg DOUBLE PRECISION,
    CONSTRAINT fk_round_temp FOREIGN KEY (round_id) REFERENCES rounds (round_id)
);

-- Table for GSR data
CREATE TABLE gsr_data (
    id SERIAL PRIMARY KEY,
    created_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT timezone('UTC', NOW()),
    event_time DOUBLE PRECISION NOT NULL,    
    unix_timestamp DOUBLE PRECISION NOT NULL,  
    lsl_timestamp DOUBLE PRECISION NOT NULL,
    player_id TEXT NOT NULL,
    round_id TEXT NOT NULL,
    raw_adc_reading SMALLINT,
    micro_siemens_reading DOUBLE PRECISION
);


CREATE TABLE trajectories (
    id SERIAL NOT NULL,
    round_id TEXT NOT NULL,
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
    unix_timestamp DOUBLE PRECISION NOT NULL,
    PRIMARY KEY (id, round_id, trial_id, unix_timestamp)
);


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
    