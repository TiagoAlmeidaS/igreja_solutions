"""
Custom exceptions for the application
"""


class DomainException(Exception):
    """Base exception for domain layer"""
    pass


class ChurchNotFoundException(DomainException):
    """Raised when church is not found"""
    pass


class ContactNotFoundException(DomainException):
    """Raised when contact is not found"""
    pass


class BroadcastNotFoundException(DomainException):
    """Raised when broadcast is not found"""
    pass


class TemplateNotFoundException(DomainException):
    """Raised when template is not found"""
    pass


class InvalidPhoneNumberException(DomainException):
    """Raised when phone number is invalid"""
    pass


class WhatsAppConfigurationException(DomainException):
    """Raised when WhatsApp configuration is invalid"""
    pass


class AuthenticationException(Exception):
    """Raised when authentication fails"""
    pass


class AuthorizationException(Exception):
    """Raised when authorization fails"""
    pass


class RepositoryException(Exception):
    """Raised when repository operation fails"""
    pass

