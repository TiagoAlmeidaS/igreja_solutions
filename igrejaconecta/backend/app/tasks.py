"""
Celery tasks for async operations
"""

from celery import Celery
from app.core.config import settings

# Initialize Celery
celery_app = Celery(
    "igrejaconecta",
    broker=settings.REDIS_URL,
    backend=settings.REDIS_URL
)

# TODO: Implement Celery tasks
# @celery_app.task
# def send_scheduled_broadcast(broadcast_id: int):
#     """Send a scheduled broadcast"""
#     pass

