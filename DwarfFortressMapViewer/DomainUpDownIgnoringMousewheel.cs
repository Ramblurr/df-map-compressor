using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DwarfFortressMapCompressor {
    class DomainUpDownIgnoringMousewheel : DomainUpDown {
        public DomainUpDownIgnoringMousewheel() : base() {
        }

        protected override void OnMouseWheel(MouseEventArgs args) {
        }
    }
}
