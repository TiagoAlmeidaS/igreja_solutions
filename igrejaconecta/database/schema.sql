-- IgrejaConecta Database Schema
-- PostgreSQL

-- Tabela: igrejas
CREATE TABLE IF NOT EXISTS churches (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    admin_name VARCHAR(100),
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    phone VARCHAR(20),
    whatsapp_phone_id TEXT,
    whatsapp_access_token TEXT, -- Encrypted
    created_at TIMESTAMP DEFAULT NOW(),
    is_active BOOLEAN DEFAULT TRUE
);

-- Tabela: contatos
CREATE TABLE IF NOT EXISTS contacts (
    id SERIAL PRIMARY KEY,
    church_id INT NOT NULL REFERENCES churches(id) ON DELETE CASCADE,
    name VARCHAR(100),
    phone VARCHAR(20) NOT NULL,
    tags TEXT[], -- ex: {'membro', 'líder'}
    created_at TIMESTAMP DEFAULT NOW(),
    CONSTRAINT unique_church_phone UNIQUE (church_id, phone)
);

-- Tabela: transmissões
CREATE TABLE IF NOT EXISTS broadcasts (
    id SERIAL PRIMARY KEY,
    church_id INT NOT NULL REFERENCES churches(id) ON DELETE CASCADE,
    title VARCHAR(100),
    message TEXT,
    link_url TEXT,
    button_text VARCHAR(50),
    contact_tags TEXT[],
    scheduled_at TIMESTAMP,
    sent_at TIMESTAMP,
    status VARCHAR(20) DEFAULT 'pending', -- pending, sent, failed
    total_sent INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT NOW()
);

-- Tabela: templates
CREATE TABLE IF NOT EXISTS templates (
    id SERIAL PRIMARY KEY,
    church_id INT NOT NULL REFERENCES churches(id) ON DELETE CASCADE,
    name VARCHAR(50) NOT NULL,
    message TEXT,
    link_url TEXT,
    button_text VARCHAR(50),
    created_at TIMESTAMP DEFAULT NOW()
);

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS idx_contacts_church_id ON contacts(church_id);
CREATE INDEX IF NOT EXISTS idx_contacts_tags ON contacts USING GIN(tags);
CREATE INDEX IF NOT EXISTS idx_broadcasts_church_id ON broadcasts(church_id);
CREATE INDEX IF NOT EXISTS idx_broadcasts_status ON broadcasts(status);
CREATE INDEX IF NOT EXISTS idx_broadcasts_scheduled_at ON broadcasts(scheduled_at);
CREATE INDEX IF NOT EXISTS idx_templates_church_id ON templates(church_id);

