using Data;

namespace Ships
{
    public class MiningDesignation : TemporalDesignation
    {
        private bool _isActive = false;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value == true)
                    _spriteRenderer.color = ColorBank.MiningDesignationActive;
                else
                    _spriteRenderer.color = ColorBank.MiningDesignationInactive;
            }
        }
    }
}