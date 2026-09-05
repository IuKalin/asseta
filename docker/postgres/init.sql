-- Extension initialization for Asseta Vault & Identity
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Initial schema creation
CREATE TABLE IF NOT EXISTS assets (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    type INT NOT NULL,
    estimated_value NUMERIC(18, 2) NOT NULL DEFAULT 0.0,
    encrypted_vault_data TEXT,
    owner_id UUID NOT NULL,
    created_at_utc TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at_utc TIMESTAMP WITH TIME ZONE,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    entity_name VARCHAR(100) NOT NULL,
    action VARCHAR(50) NOT NULL,
    performed_by UUID NOT NULL,
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    details JSONB
);

-- Indices
CREATE INDEX IF NOT EXISTS idx_assets_owner ON assets(owner_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_audit_timestamp ON audit_logs(timestamp);
