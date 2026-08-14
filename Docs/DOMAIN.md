# MafSubAgent — Domain Knowledge

## What this is

MafSubAgent is a chatbot that acts as a university advisor. A person chats
with it in natural language and it answers questions about academic
planning, course selection, university policies, deadlines, and other
student resources. It doesn't look anything up in a university's real
systems — it answers from an Azure OpenAI language model that has been
given a short set of instructions describing the advisor persona, and it
is explicitly told to tell the user to double-check anything
university-specific with the actual university rather than presenting
itself as an authoritative source. The chatbot is hosted and exposed to
end users through the Xians agent platform, which this codebase registers
itself with as a single conversational agent.

## Who uses it

There is no authentication, authorization, or role model anywhere in this
codebase — no roles enum, no `[Authorize]`-style checks, no permission
config. The code simply registers one agent (`Program.cs:27-31`) and
answers any incoming chat message the same way regardless of who sent it
(`Program.cs:41-45`). In practice that means:

- **End user ("student")** — anyone able to send a message to this agent
  through the Xians platform. They can ask questions and get advice; they
  cannot configure the agent, see other users' conversations, or do
  anything beyond chatting.

Whether the Xians platform itself restricts *who* can reach this agent
(e.g. only enrolled students, only authenticated platform users) is
enforced outside this repository, if at all — see "Needs product input."

## Core concepts

- **MafSubAgent** — the wrapper class (`MafSubAgent.cs:6`) that owns the
  actual AI agent. It holds the Azure OpenAI connection details and the
  persona instructions, and exposes a single `RunAsync(message)` method
  that takes one user message and returns one text reply. It has no
  memory of its own beyond what the underlying `AIAgent` keeps.
- **Xians Platform / Xians Agent** — the external hosting platform this
  process registers itself with (`Program.cs:20-31`). The platform is
  responsible for actually receiving user chat messages and delivering
  replies; this repo only defines what to do with a message once handed
  one.
- **Supervisor Workflow** — a built-in conversational workflow
  (`Program.cs:35`, `DefineSupervisor()`) that the Xians platform provides.
  It's the thing that actually wires "a chat message arrived" to the
  handler defined in this code. Its internal behavior (routing, multi-turn
  history, etc.) lives in the Xians platform/library, not in this repo.
- **User message / reply** — one inbound chat message
  (`context.Message.Text`) produces exactly one outbound reply
  (`context.ReplyAsync(response)`), via `MafSubAgent.RunAsync`
  (`Program.cs:41-45`). There is no concept of tickets, sessions, or
  multi-step cases in this code — each message is handled independently
  by the handler shown here.

## Business rules

There is no procedural business logic (no validation functions, state
machines, or calculations) in this codebase. The rules that shape the
product's behavior are almost entirely instructions given to the language
model, plus a small set of startup configuration requirements:

- The advisor persona must stay in the university-advisor domain: academic
  planning, course selection, university policies, deadlines, and student
  resources (`MafSubAgent.cs:21-23`). This is a prompt instruction, not an
  enforced filter — nothing in the code stops the model from answering
  off-topic questions if the model chooses to.
- Replies must be concise and easy to understand (`MafSubAgent.cs:24`).
- When the advisor is unsure about a university-specific policy or
  requirement, it must say so and tell the student to verify with the
  university directly, rather than presenting a guess as fact
  (`MafSubAgent.cs:25`). Again, this is instructed behavior, not something
  the code checks or enforces after the model responds.
- The application refuses to start unless five environment variables are
  all present: `AZURE_OPENAI_ENDPOINT`, `AZURE_OPENAI_API_KEY`,
  `AZURE_OPENAI_DEPLOYMENT`, `XIANS_SERVER_URL`, and `XIANS_API_KEY`
  (`Program.cs:8-17`) — each throws an `InvalidOperationException` if
  missing. This is the one genuinely enforced rule in the codebase.
- The agent registers itself with `IsTemplate = false` (`Program.cs:30`),
  meaning it registers as a live/runnable agent rather than a template
  (see "Needs product input" — the distinction's effect isn't visible in
  this repo).

## User flows

There is no frontend in this repository — no pages, routes, or UI
components. The only user-facing surface is the chat conversation itself,
mediated entirely by the Xians platform. The one flow this code
implements is:

1. A user sends a chat message through whatever front-end/channel the
   Xians platform exposes (outside this repo).
2. The Xians Supervisor Workflow receives it and invokes the handler
   registered in `Program.cs:41-45`.
3. The handler passes the raw message text to
   `MafSubAgent.RunAsync(message)` (`MafSubAgent.cs:31-35`).
4. `MafSubAgent` forwards the message to the Azure OpenAI chat deployment
   configured via `AZURE_OPENAI_ENDPOINT` / `AZURE_OPENAI_DEPLOYMENT`,
   along with the university-advisor persona instructions.
5. The model's text response is returned up the chain and sent back to
   the user via `context.ReplyAsync(response)`.

Anything beyond this — how the Xians platform renders the conversation,
handles multiple concurrent users, or persists chat history — is
determined by the Xians platform itself and can't be traced from this
codebase.

## Glossary

- **MAF** — Microsoft Agent Framework; the `Microsoft.Agents.AI` library
  used to wrap a chat client as an `AIAgent`.
- **AIAgent** — the Microsoft Agent Framework abstraction representing a
  runnable conversational agent with a name, options, and instructions.
- **ChatClientAgentOptions / ChatOptions** — configuration objects used to
  name the agent and supply its system instructions to the underlying
  chat client.
- **Xians Platform** — the external hosting/runtime service
  (`Xians.Lib.Agents.Core`) that this process connects to and registers
  an agent with.
- **Supervisor Workflow** — a built-in Xians workflow type for handling
  ordinary back-and-forth chat conversations (as opposed to other,
  non-conversational workflow types the platform may support).
- **Deployment (Azure OpenAI)** — the named model deployment on the
  Azure OpenAI resource that the agent sends prompts to
  (`AZURE_OPENAI_DEPLOYMENT`).
- **IsTemplate** — a registration flag distinguishing a live agent
  instance from a reusable template; set to `false` here.

## Needs product input

- **No access control found**: nothing in this repo restricts who can
  chat with the advisor. If this is meant to be limited to a specific
  university's enrolled students, that boundary must be enforced by the
  Xians platform or another layer outside this repository — it's not
  visible here, and it's unclear whether that enforcement exists at all.
- **`IsTemplate = false`** (`Program.cs:30`) — the code comment says "See
  important notes below" but no such notes exist in this file or repo.
  The actual implication of template vs. non-template registration on
  domain behavior (e.g. can multiple advisor instances run, is state
  shared) is not explained anywhere in the codebase.
- **No conversation memory/session model visible**: whether a student's
  earlier messages in the same conversation are remembered when answering
  a later message is controlled by the Xians Supervisor Workflow and/or
  the underlying `AIAgent`, not by any code in this repo. Can't confirm
  behavior either way from what's here.
- **No guardrails on model output**: the "verify with the university"
  disclaimer (`MafSubAgent.cs:25`) is instructed, not enforced — there's
  no check that the model actually included it, and no filter on what
  topics the model will or won't engage with. Whether that's an
  acceptable risk for a real advising product is a product decision, not
  a coding fact this doc can settle.
