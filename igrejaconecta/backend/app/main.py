"""
IgrejaConecta - Backend API
FastAPI application entry point
"""

from fastapi import FastAPI, Request
from fastapi.middleware.cors import CORSMiddleware
from app.presentation.api.v1 import church, contacts, broadcasts, templates
from app.presentation.middleware.error_handler import exception_handler
from app.core.exceptions import (
    DomainException,
    AuthenticationException,
    AuthorizationException,
    RepositoryException,
)

app = FastAPI(
    title="IgrejaConecta API",
    description="MicroSaaS para Automação de Avisos e Transmissões via WhatsApp",
    version="1.0.0"
)

# CORS Configuration
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Configure according to your frontend URL
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Exception handlers
app.add_exception_handler(DomainException, exception_handler)
app.add_exception_handler(AuthenticationException, exception_handler)
app.add_exception_handler(AuthorizationException, exception_handler)
app.add_exception_handler(RepositoryException, exception_handler)
app.add_exception_handler(Exception, exception_handler)


@app.get("/")
async def root():
    return {"message": "IgrejaConecta API", "version": "1.0.0"}


@app.get("/health")
async def health_check():
    return {"status": "healthy"}


# Include routers
app.include_router(church.router, prefix="/api/v1/church", tags=["church"])
app.include_router(contacts.router, prefix="/api/v1/contacts", tags=["contacts"])
app.include_router(broadcasts.router, prefix="/api/v1/broadcasts", tags=["broadcasts"])
app.include_router(templates.router, prefix="/api/v1/templates", tags=["templates"])

