FROM python:3.12.7-slim

ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1

WORKDIR /worker

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY main.py .
COPY app ./app

# No secrets in the image: docker-compose passes .env through env_file at runtime.
RUN useradd --create-home --uid 10001 worker
USER worker

CMD ["python", "main.py"]
