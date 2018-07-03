local addonName = 'FishingHelper'
local cnt0 = 0
local fishingBegin = false

local function onSlotUpdate(event, bagId, slotIndex, isNew)
  local lure = GetFishingLure()
  local cnt = 0
  if lure then
    cnt = select(3, GetFishingLureInfo(lure))
  else
    cnt = 0
  end
  if( (not isNew and (GetItemType(bagId, slotIndex) == ITEMTYPE_LURE)) and (cnt0 - cnt == 1) )then
    FishingWindowRect:SetEdgeColor(0, 255, 0, 1)
    FishingWindowRect:SetCenterColor(0, 255, 0, 1)
  end
  cnt0 = cnt
end

local function onLureCleared(event)
  -- d("FV:onLureCleared")
  local lure = GetFishingLure()
  if lure then
    cnt0 = select(3, GetFishingLureInfo(lure))
  end
end

local function onLureSet(event,lure)
  -- d("FV:onLureSet")
  if lure then
    cnt0 = select(3, GetFishingLureInfo(lure))
  end
end

local function onChatterEnd(eventCode)
  if fishingBegin then
    FishingWindowRect:SetEdgeColor(0, 0, 255, 1)
    FishingWindowRect:SetCenterColor(0, 0, 255, 1)
  end
end

local function onNoInteractTarget(eventCode)
  FishingWindowRect:SetEdgeColor(0, 0, 0, 1)
  FishingWindowRect:SetCenterColor(0, 0, 0, 1)
  fishingBegin = false
end

local function onClientInteractResult(eventCode, result, interactTargetName)
  if result == CLIENT_INTERACT_RESULT_SUCCESS and interactTargetName == "Fishing Hole" then
    FishingWindowRect:SetEdgeColor(255, 0, 0, 1)
    FishingWindowRect:SetCenterColor(255, 0, 0, 1)
	fishingBegin = true
  else
    FishingWindowRect:SetEdgeColor(0, 0, 0, 1)
    FishingWindowRect:SetCenterColor(0, 0, 0, 1)
	fishingBegin = false
  end
end

local function eventRegister()
  EVENT_MANAGER:RegisterForEvent(addonName, EVENT_INVENTORY_SINGLE_SLOT_UPDATE, onSlotUpdate)
  
  EVENT_MANAGER:RegisterForEvent(addonName, EVENT_FISHING_LURE_CLEARED, onLureCleared)
  EVENT_MANAGER:RegisterForEvent(addonName, EVENT_FISHING_LURE_SET, onLureSet)

  EVENT_MANAGER:RegisterForEvent(addonName, EVENT_CHATTER_END, onChatterEnd)
  EVENT_MANAGER:RegisterForEvent(addonName, EVENT_NO_INTERACT_TARGET, onNoInteractTarget)
  EVENT_MANAGER:RegisterForEvent(addonName, EVENT_CLIENT_INTERACT_RESULT, onClientInteractResult)
end

local function onLoaded( event, addon )
  if ( addon ~= addonName ) then return end
    EVENT_MANAGER:UnregisterForEvent(addonName, EVENT_ADD_ON_LOADED)
    eventRegister()
  local lure = GetFishingLure()
  if lure then
    cnt0 = select(3, GetFishingLureInfo(lure))
  end

end

EVENT_MANAGER:RegisterForEvent(addonName, EVENT_ADD_ON_LOADED,onLoaded)
