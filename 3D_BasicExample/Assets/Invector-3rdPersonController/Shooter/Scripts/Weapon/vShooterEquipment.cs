using Invector.vShooter;
using UnityEngine;

namespace Invector.vItemManager
{
    [vClassHeader("Shooter Equipment", openClose = false, useHelpBox = true, helpBoxText = "Use this component if you also use the ItemManager in your Character")]
    public class vShooterEquipment : vMeleeEquipment
    {
        protected vShooterWeapon _shooter;
        protected vEquipment _secundaryEquipment;
     
        protected bool withoutShooterWeapon;       

        public virtual vEquipment secundaryEquipment
        {
            get
            {                
                return _secundaryEquipment;
            }
        }
        public virtual vShooterWeapon shooterWeapon
        {
            get
            {
                if (!_shooter && !withoutShooterWeapon)
                {
                    _shooter = GetComponent<vShooterWeapon>();
                    if (!_shooter) withoutShooterWeapon = true;
                }

                return _shooter;
            }
        }
     
        public override void OnEquip(vItem item)
        {
          
            if (shooterWeapon)
            {
                shooterWeapon.changeAmmoHandle = new vShooterWeapon.ChangeAmmoHandle(ChangeAmmo);
                shooterWeapon.checkAmmoHandle = new vShooterWeapon.CheckAmmoHandle(CheckAmmo);
                var damageAttribute = item.GetItemAttribute(vItemAttributes.Damage);

                if (damageAttribute != null)
                {
                    shooterWeapon.maxDamage = damageAttribute.value;
                }

                if (secundaryEquipment)
                {
                    secundaryEquipment.OnEquip(item);                    
                }
            }
            base.OnEquip(item);
        }

        public override void OnUnequip(vItem item)
        {
            if (shooterWeapon)
            {
                shooterWeapon.changeAmmoHandle = null;
                shooterWeapon.checkAmmoHandle = null;

                if (secundaryEquipment)
                {
                    secundaryEquipment.OnUnequip(item);
                }
            }         
           
            base.OnUnequip(item);
        }

        protected virtual bool CheckAmmo(ref bool isValid, ref int totalAmmo)
        {
            if (!referenceItem) return false;
            var ammoAttribute = referenceItem.GetItemAttribute(vItemAttributes.AmmoCount);
            isValid = ammoAttribute != null && !ammoAttribute.isBool;
            if (isValid) totalAmmo = ammoAttribute.value;
            return isValid && ammoAttribute.value > 0;
        }

        protected virtual void ChangeAmmo(int value)
        {
            if (!referenceItem) return;
            var ammoAttribute = referenceItem.GetItemAttribute(vItemAttributes.AmmoCount);

            if (ammoAttribute != null)
            {
                ammoAttribute.value += value;
            }
        }

    }
}