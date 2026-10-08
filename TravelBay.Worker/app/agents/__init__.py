"""The two AI agents. Each processes every destination still missing its data and reports into a RunReporter."""


class FatalAgentError(RuntimeError):
    """Stops the whole run (missing or rejected API key); the message is shown in the run log."""
