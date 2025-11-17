"""
Faker fixtures for tests
"""

from faker import Faker
from datetime import datetime, timedelta
from app.domain.entities.church import Church
from app.domain.entities.contact import Contact
from app.domain.entities.broadcast import Broadcast, BroadcastStatus
from app.domain.entities.template import Template
from app.domain.value_objects.phone import Phone

fake = Faker('pt_BR')


def fake_church(**kwargs) -> Church:
    """Create a fake church entity"""
    return Church(
        id=kwargs.get('id', None),
        name=kwargs.get('name', fake.company()),
        admin_name=kwargs.get('admin_name', fake.name()),
        email=kwargs.get('email', fake.email()),
        phone=kwargs.get('phone', fake.phone_number()),
        whatsapp_phone_id=kwargs.get('whatsapp_phone_id', None),
        whatsapp_access_token=kwargs.get('whatsapp_access_token', None),
        created_at=kwargs.get('created_at', datetime.utcnow()),
        is_active=kwargs.get('is_active', True)
    )


def fake_contact(church_id: int, **kwargs) -> Contact:
    """Create a fake contact entity"""
    return Contact(
        id=kwargs.get('id', None),
        church_id=church_id,
        name=kwargs.get('name', fake.name()),
        phone=kwargs.get('phone', Phone(fake.phone_number())),
        tags=kwargs.get('tags', []),
        created_at=kwargs.get('created_at', datetime.utcnow())
    )


def fake_broadcast(church_id: int, **kwargs) -> Broadcast:
    """Create a fake broadcast entity"""
    return Broadcast(
        id=kwargs.get('id', None),
        church_id=church_id,
        title=kwargs.get('title', fake.sentence()),
        message=kwargs.get('message', fake.text()),
        link_url=kwargs.get('link_url', fake.url()),
        button_text=kwargs.get('button_text', 'Assistir Agora'),
        contact_tags=kwargs.get('contact_tags', []),
        scheduled_at=kwargs.get('scheduled_at', None),
        sent_at=kwargs.get('sent_at', None),
        status=kwargs.get('status', BroadcastStatus.PENDING),
        total_sent=kwargs.get('total_sent', 0),
        created_at=kwargs.get('created_at', datetime.utcnow())
    )


def fake_template(church_id: int, **kwargs) -> Template:
    """Create a fake template entity"""
    return Template(
        id=kwargs.get('id', None),
        church_id=church_id,
        name=kwargs.get('name', fake.word()),
        message=kwargs.get('message', fake.text()),
        link_url=kwargs.get('link_url', fake.url()),
        button_text=kwargs.get('button_text', 'Assistir Agora'),
        created_at=kwargs.get('created_at', datetime.utcnow())
    )

