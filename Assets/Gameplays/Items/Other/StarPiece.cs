using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPiece : ItemCommon
{
    public override void GotIt(StatusSaver status) {
        status.StarPieces++;
        Destroy(gameObject);
    }
}
