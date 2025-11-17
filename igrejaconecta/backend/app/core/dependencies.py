"""
Dependency injection for the application
"""

from typing import Generator
from sqlalchemy.orm import Session
from app.infrastructure.database.database import SessionLocal, get_db_session
from app.infrastructure.database.repositories.church_repository_impl import ChurchRepositoryImpl
from app.infrastructure.database.repositories.contact_repository_impl import ContactRepositoryImpl
from app.infrastructure.database.repositories.broadcast_repository_impl import BroadcastRepositoryImpl
from app.infrastructure.database.repositories.template_repository_impl import TemplateRepositoryImpl
from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.interfaces.repositories.contact_repository import IContactRepository
from app.application.interfaces.repositories.broadcast_repository import IBroadcastRepository
from app.application.interfaces.repositories.template_repository import ITemplateRepository
from app.infrastructure.external.whatsapp.whatsapp_client import WhatsAppClient
from app.infrastructure.external.firebase.firebase_auth import FirebaseAuth
from app.application.interfaces.services.whatsapp_service import IWhatsAppService
from app.application.interfaces.services.firebase_service import IFirebaseService


def get_db() -> Generator[Session, None, None]:
    """Dependency for getting database session"""
    yield from get_db_session()


def get_church_repository(db: Session = None) -> IChurchRepository:
    """Dependency for church repository"""
    if db is None:
        db = next(get_db())
    return ChurchRepositoryImpl(db)


def get_contact_repository(db: Session = None) -> IContactRepository:
    """Dependency for contact repository"""
    if db is None:
        db = next(get_db())
    return ContactRepositoryImpl(db)


def get_broadcast_repository(db: Session = None) -> IBroadcastRepository:
    """Dependency for broadcast repository"""
    if db is None:
        db = next(get_db())
    return BroadcastRepositoryImpl(db)


def get_template_repository(db: Session = None) -> ITemplateRepository:
    """Dependency for template repository"""
    if db is None:
        db = next(get_db())
    return TemplateRepositoryImpl(db)


def get_whatsapp_service() -> IWhatsAppService:
    """Dependency for WhatsApp service"""
    return WhatsAppClient()


def get_firebase_service() -> IFirebaseService:
    """Dependency for Firebase service"""
    return FirebaseAuth()

