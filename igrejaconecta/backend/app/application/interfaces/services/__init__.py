"""
Service interfaces
"""

from app.application.interfaces.services.whatsapp_service import IWhatsAppService
from app.application.interfaces.services.firebase_service import IFirebaseService

__all__ = ["IWhatsAppService", "IFirebaseService"]

