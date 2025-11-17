"""
Repository interfaces
"""

from app.application.interfaces.repositories.church_repository import IChurchRepository
from app.application.interfaces.repositories.contact_repository import IContactRepository
from app.application.interfaces.repositories.broadcast_repository import IBroadcastRepository
from app.application.interfaces.repositories.template_repository import ITemplateRepository

__all__ = [
    "IChurchRepository",
    "IContactRepository",
    "IBroadcastRepository",
    "ITemplateRepository",
]

