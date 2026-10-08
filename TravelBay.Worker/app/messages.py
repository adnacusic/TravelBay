"""RabbitMQ contract shared with the API (TravelBay.Model/Constants/AiAgentMessages.cs) and the
int values of the C# enums AiAgentType / AiAgentRunStatus stored in the AiAgentRuns table."""

from enum import IntEnum

QUEUE_NAME = "travelbay.ai-agents"

GENERATE_KEYWORDS = "GenerateKeywords"
FIND_IMAGES = "FindImages"


class AgentType(IntEnum):
    KEYWORDS = 0
    IMAGES = 1


class RunStatus(IntEnum):
    QUEUED = 0
    RUNNING = 1
    COMPLETED = 2
    FAILED = 3


COMMAND_AGENT = {
    GENERATE_KEYWORDS: AgentType.KEYWORDS,
    FIND_IMAGES: AgentType.IMAGES,
}
