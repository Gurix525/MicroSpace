namespace Ships
{
    public class CancelDesignation : TemporalDesignation
    {
        private bool _isActive = false;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value == true)
                    _spriteRenderer.color = _colors.CancelDesignationActive;
                else
                    _spriteRenderer.color = _colors.CancelDesignationInactive;
            }
        }
    }
}