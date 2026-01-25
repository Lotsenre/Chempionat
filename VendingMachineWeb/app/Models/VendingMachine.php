<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Concerns\HasUuids;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\HasOne;

class VendingMachine extends Model
{
    use HasUuids;

    protected $keyType = 'string';
    public $incrementing = false;

    protected $fillable = [
        'id',
        'serial_number',
        'name',
        'model',
        'status',
        'location',
        'place',
        'coordinates',
        'install_date',
        'last_maintenance_date',
        'working_hours',
        'timezone',
        'total_income',
        'user_id',
        'work_mode',
        'rfid_cash_collection',
        'rfid_loading',
        'rfid_service',
        'kit_online_id',
        'company',
        'payment_type',
        'critical_threshold_template',
        'service_priority',
        'manager',
        'notification_template',
        'engineer',
        'operator',
        'technician',
        'notes',
    ];

    protected $casts = [
        'install_date' => 'datetime',
        'last_maintenance_date' => 'datetime',
        'total_income' => 'decimal:2',
    ];

    public function modem(): HasOne
    {
        return $this->hasOne(Modem::class);
    }
}
