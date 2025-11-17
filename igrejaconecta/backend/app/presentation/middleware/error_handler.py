"""
Error handler middleware
"""

from fastapi import Request, status
from fastapi.responses import JSONResponse
from app.core.exceptions import (
    DomainException,
    ChurchNotFoundException,
    ContactNotFoundException,
    BroadcastNotFoundException,
    TemplateNotFoundException,
    AuthenticationException,
    AuthorizationException,
    RepositoryException,
    InvalidPhoneNumberException,
    WhatsAppConfigurationException,
)


async def exception_handler(request: Request, exc: Exception):
    """Global exception handler"""
    
    # Domain exceptions
    if isinstance(exc, ChurchNotFoundException):
        return JSONResponse(
            status_code=status.HTTP_404_NOT_FOUND,
            content={"detail": str(exc)}
        )
    
    if isinstance(exc, ContactNotFoundException):
        return JSONResponse(
            status_code=status.HTTP_404_NOT_FOUND,
            content={"detail": str(exc)}
        )
    
    if isinstance(exc, BroadcastNotFoundException):
        return JSONResponse(
            status_code=status.HTTP_404_NOT_FOUND,
            content={"detail": str(exc)}
        )
    
    if isinstance(exc, TemplateNotFoundException):
        return JSONResponse(
            status_code=status.HTTP_404_NOT_FOUND,
            content={"detail": str(exc)}
        )
    
    # Authentication/Authorization
    if isinstance(exc, AuthenticationException):
        return JSONResponse(
            status_code=status.HTTP_401_UNAUTHORIZED,
            content={"detail": str(exc)}
        )
    
    if isinstance(exc, AuthorizationException):
        return JSONResponse(
            status_code=status.HTTP_403_FORBIDDEN,
            content={"detail": str(exc)}
        )
    
    # Validation exceptions
    if isinstance(exc, InvalidPhoneNumberException):
        return JSONResponse(
            status_code=status.HTTP_400_BAD_REQUEST,
            content={"detail": str(exc)}
        )
    
    if isinstance(exc, WhatsAppConfigurationException):
        return JSONResponse(
            status_code=status.HTTP_400_BAD_REQUEST,
            content={"detail": str(exc)}
        )
    
    # Repository exceptions
    if isinstance(exc, RepositoryException):
        return JSONResponse(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            content={"detail": str(exc)}
        )
    
    # Domain exceptions (generic)
    if isinstance(exc, DomainException):
        return JSONResponse(
            status_code=status.HTTP_400_BAD_REQUEST,
            content={"detail": str(exc)}
        )
    
    # Generic exception
    return JSONResponse(
        status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
        content={"detail": "Internal server error"}
    )

