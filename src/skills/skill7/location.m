c = 340;
loc = [1 5.001 1];
h = loc(3); % Beacon height from the ground
mics = [0 0 2; 0 5 2; 5 0 2; 5 5 2; 2.5 2.5 3];
t = [];
for i = 1:size(mics, 1)
    t(i) = sqrt(sum((mics(i,:) - loc).^2)) / c;
end
y = [];
k = 1;

for i = 1:size(mics, 1)
    for j = 1:size(mics, 1)
        if i == j || i < j
            continue; % Do not compare with own time or twice thesame
        end
        
        mic1 = mics(i,:);
        mic2 = mics(j,:);
        
        t1 = t(i);
        t2 = t(j);

        d = sqrt((mic1(1) - mic2(1))^2 + (mic1(2) - mic2(2))^2);

        h1 = mic1(3) - h;
        h2 = mic2(3) - h;

        % Convert to time in 2D plane
        t1_ = sqrt((t1 * c)^2 - h1^2) / c;
        t2_ = sqrt((t2 * c)^2 - h2^2) / c;

        d1 = ((t1_ * c)^2 - (t2_ * c)^2 + d^2) / (2 * d); %  - h1^2 + h2^2
        x = sqrt((t1_ * c)^2 - d1^2);
        mx = d1/d * (mic2(1) - mic1(1));
        my = d1/d * (mic2(2) - mic1(2));

        r = [sqrt(mx^2 + my^2), x];
        %angle = atan2((mic2(2) - mic1(2)), (mic2(1) - mic1(1)))
        angle = -atan2(my, mx);
        rot = [cos(angle) -sin(angle); sin(angle) cos(angle)];
        y(k,:) = r * rot + [mic1(1) mic1(2)];
        k = k + 1;
        
        r = [sqrt(mx^2 + my^2), -x];
        %angle = atan2((mic2(2) - mic1(2)), (mic2(1) - mic1(1)));
        angle = -atan2(my, mx);
        rot = [cos(angle) -sin(angle); sin(angle) cos(angle)];
        y(k,:) = r * rot + [mic1(1) mic1(2)];
        k = k + 1;
    end
end

% Perform least square error test
error1 = 0;
error2 = 0;
locs1 = [y(1,:)];
locs2 = [y(2,:)];

for j = 3:2:size(y,1)
    i = ceil(j/2);
    if sum((y(j,:) - y(1,:)).^2) < sum((y(j+1,:) - y(1,:)).^2)
       locs1(i,:) = y(j,:);
    else
       locs1(i,:) = y(j+1,:);
    end
    error1 = error1 + sum((locs1(i,:) - y(1,:)).^2);
end

for j = 3:2:size(y,1)
    i = ceil(j/2);
    if sum((y(j) - y(2)).^2) < sum((y(j+1) - y(2)).^2)
       locs2(i,:) = y(j,:);
    else
       locs2(i,:) = y(j+1,:);
    end
    error2 = error2 + sum((locs2(i,:) - y(2,:)).^2);
end

%locs1 = locs1
%locs2 = locs2
%error1
%error2
if error1 < error2
   loc = real(mean(locs1)) % Prevent the 0.0000i from showing 
   error = error1
else
    loc = real(mean(locs2)) % Prevent the 0.0000i from showing 
    error = error2
end