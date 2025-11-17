"""
Security utilities for authentication and encryption
"""

from datetime import datetime, timedelta
from typing import Optional
from jose import JWTError, jwt
from passlib.context import CryptContext
from cryptography.fernet import Fernet
from app.core.config import settings

# Password hashing
pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")

# Token encryption
# TODO: Generate and store Fernet key securely
# fernet_key = Fernet.generate_key()
# cipher_suite = Fernet(fernet_key)


def verify_password(plain_password: str, hashed_password: str) -> bool:
    """Verify a password against its hash"""
    return pwd_context.verify(plain_password, hashed_password)


def get_password_hash(password: str) -> str:
    """Hash a password"""
    return pwd_context.hash(password)


def create_access_token(data: dict, expires_delta: Optional[timedelta] = None) -> str:
    """Create a JWT access token"""
    to_encode = data.copy()
    if expires_delta:
        expire = datetime.utcnow() + expires_delta
    else:
        expire = datetime.utcnow() + timedelta(minutes=settings.JWT_EXPIRE_MINUTES)
    
    to_encode.update({"exp": expire})
    encoded_jwt = jwt.encode(to_encode, settings.SECRET_KEY, algorithm=settings.JWT_ALGORITHM)
    return encoded_jwt


def decode_access_token(token: str) -> Optional[dict]:
    """Decode and verify a JWT token"""
    try:
        payload = jwt.decode(token, settings.SECRET_KEY, algorithms=[settings.JWT_ALGORITHM])
        return payload
    except JWTError:
        return None


def encrypt_token(token: str) -> str:
    """Encrypt a token using Fernet"""
    # TODO: Implement token encryption
    # return cipher_suite.encrypt(token.encode()).decode()
    return token  # Placeholder


def decrypt_token(encrypted_token: str) -> str:
    """Decrypt a token using Fernet"""
    # TODO: Implement token decryption
    # return cipher_suite.decrypt(encrypted_token.encode()).decode()
    return encrypted_token  # Placeholder

