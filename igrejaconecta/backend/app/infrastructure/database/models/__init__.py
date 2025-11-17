"""
SQLAlchemy models initialization
"""

from app.infrastructure.database.models.church_model import ChurchModel
from app.infrastructure.database.models.contact_model import ContactModel
from app.infrastructure.database.models.broadcast_model import BroadcastModel
from app.infrastructure.database.models.template_model import TemplateModel

# Import all models for Alembic
__all__ = ["ChurchModel", "ContactModel", "BroadcastModel", "TemplateModel"]
