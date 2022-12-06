using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public abstract class RigidEntity : Entity
    {
        public override int Id { get; protected set; }
    }
}