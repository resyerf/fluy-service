namespace Fluy.Domain.Enums;

/// <summary>
/// Solo Draft/Submitted tienen comportamiento implementado por ahora (primer vertical slice,
/// CODE.md §11). El resto existe en el enum porque son estados reales del flujo de CLAUDE.md §3,
/// pero las transiciones hacia ellos las conducirá el Workflow/Approval Engine, todavía no
/// implementado — no se fuerza esa lógica antes de tener el motor real.
/// </summary>
public enum RequestStatus
{
    Draft = 0,
    Submitted = 1,
    InReview = 2,
    Approved = 3,
    Rejected = 4,
    ReturnedForCorrection = 5,
    Completed = 6,
    Cancelled = 7
}
