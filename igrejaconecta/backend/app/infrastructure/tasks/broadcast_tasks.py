"""
Celery tasks for broadcast operations
"""

from celery import Celery
from app.core.config import settings
from app.infrastructure.database.database import SessionLocal
from app.infrastructure.database.repositories.broadcast_repository_impl import BroadcastRepositoryImpl
from app.infrastructure.database.repositories.contact_repository_impl import ContactRepositoryImpl
from app.infrastructure.database.repositories.church_repository_impl import ChurchRepositoryImpl
from app.infrastructure.external.whatsapp.whatsapp_client import WhatsAppClient
from app.domain.entities.broadcast import BroadcastStatus
from datetime import datetime

# Initialize Celery
celery_app = Celery(
    "igrejaconecta",
    broker=settings.REDIS_URL,
    backend=settings.REDIS_URL
)

celery_app.conf.update(
    task_serializer='json',
    accept_content=['json'],
    result_serializer='json',
    timezone='UTC',
    enable_utc=True,
)


@celery_app.task
def send_scheduled_broadcast(broadcast_id: int):
    """Send a scheduled broadcast"""
    db = SessionLocal()
    try:
        broadcast_repo = BroadcastRepositoryImpl(db)
        contact_repo = ContactRepositoryImpl(db)
        church_repo = ChurchRepositoryImpl(db)
        whatsapp_service = WhatsAppClient()
        
        # Get broadcast
        broadcast = broadcast_repo.get_by_id(broadcast_id)
        if not broadcast:
            return {"error": "Broadcast not found"}
        
        # Check if can be sent
        if not broadcast.can_be_sent():
            return {"error": f"Cannot send broadcast with status {broadcast.status.value}"}
        
        # Get church
        church = church_repo.get_by_id(broadcast.church_id)
        if not church or not church.is_whatsapp_configured():
            return {"error": "WhatsApp not configured"}
        
        # Get contacts
        contacts = []
        if broadcast.contact_tags:
            contacts = contact_repo.list_by_tags(broadcast.church_id, broadcast.contact_tags)
        else:
            contacts = contact_repo.list_by_church(broadcast.church_id, limit=10000)
        
        # Send messages
        success_count = 0
        failed_count = 0
        
        for contact in contacts:
            try:
                if broadcast.link_url and broadcast.button_text:
                    whatsapp_service.send_interactive_message(
                        to=contact.phone.value,
                        body=broadcast.message,
                        button_text=broadcast.button_text,
                        url=broadcast.link_url,
                        phone_id=church.whatsapp_phone_id,
                        token=church.whatsapp_access_token
                    )
                else:
                    whatsapp_service.send_text_message(
                        to=contact.phone.value,
                        message=broadcast.message,
                        phone_id=church.whatsapp_phone_id,
                        token=church.whatsapp_access_token
                    )
                success_count += 1
            except Exception:
                failed_count += 1
        
        # Update broadcast
        broadcast.update_total_sent(success_count)
        broadcast.send()
        broadcast_repo.update(broadcast)
        
        return {
            "success": success_count,
            "failed": failed_count,
            "total": len(contacts)
        }
    finally:
        db.close()


@celery_app.task
def process_scheduled_broadcasts():
    """Process all scheduled broadcasts that are due"""
    db = SessionLocal()
    try:
        broadcast_repo = BroadcastRepositoryImpl(db)
        
        # Get scheduled broadcasts due now
        scheduled = broadcast_repo.list_scheduled(before=datetime.utcnow())
        
        results = []
        for broadcast in scheduled:
            result = send_scheduled_broadcast.delay(broadcast.id)
            results.append({"broadcast_id": broadcast.id, "task_id": result.id})
        
        return {"processed": len(results), "tasks": results}
    finally:
        db.close()

